using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service2.DTO;
using Service2.Interface;
using Service2.Model;

namespace Service2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransaction transRepo;
        private readonly generalRes res;
        private readonly IHttpClientFactory httpClient;

        public TransactionController(ITransaction transRepo, generalRes res, IHttpClientFactory httpClient)
        {
            this.transRepo = transRepo;
            this.res = res;
            this.httpClient = httpClient;
        }
        [HttpPost]
        public async Task<IActionResult> sendMoney([FromForm]CreateTrans modelreq)
        {
            string container = "";
            if (modelreq.amount <= 0)
            {
                res.Message = "Failed";
                res.Data = "Amount must be greater than 0";
                return StatusCode(StatusCodes.Status400BadRequest, res);
            }

            var httpClientUser = httpClient.CreateClient("User");
            var receiverResponse = await httpClientUser
                .GetAsync($"http://service1.runasp.net/api/User/{modelreq.receiverPhone}");

            if (receiverResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                res.Message = "Failed";
                res.Data = "Receiver not found";
                return StatusCode(StatusCodes.Status404NotFound, res);
            }
            var rawJson = await receiverResponse.Content.ReadAsStringAsync();
            Console.WriteLine(rawJson);

            var receiverContent = await receiverResponse.Content.ReadFromJsonAsync<apiResponse<userdata>>();
            var receiverId = receiverContent.Data.Id;
            container = receiverId;
            if(Guid.Parse(container)==modelreq.sender)
            {
                res.Message = "Failed";
                res.Data = "Can't send to your phone number";
                return StatusCode(StatusCodes.Status400BadRequest, res);
            }
            var updatePayload = new
            {
                modelreq.amount
            };

            var updateResponse = await httpClientUser
                .PutAsJsonAsync($"http://service1.runasp.net/api/User/{receiverId}", updatePayload);
            if (!updateResponse.IsSuccessStatusCode)
            {
                res.Message = "Failed";
                res.Data = "Failed to update receiver balance";
                return StatusCode((int)updateResponse.StatusCode, res);
            }
            var debitPayload = new { Amount = -modelreq.amount };
            var debitResponse = await httpClientUser.PutAsJsonAsync($"http://service1.runasp.net/api/User/{modelreq.sender}", debitPayload);
            if (!debitResponse.IsSuccessStatusCode)
            {
                res.Message = "Failed";
                res.Data = "Failed to debit sender balance";
                return StatusCode((int)debitResponse.StatusCode, res);
            }
            var payload = new
            {
                SenderId = modelreq.sender,
                ReceiverId = Guid.Parse(container),
                TransactionDate = DateTime.Now,
                Amount = modelreq.amount,
                TransactionType = "Send"
            };
            var response = await httpClientUser.PostAsJsonAsync($"http://service3.runasp.net/api/Report", payload);
            if(!response.IsSuccessStatusCode)
            {
                res.Message = "Failed";
                res.Data = "Failed to save data in report";
                return StatusCode((int)response.StatusCode, res);
            }

            var trans = new Transactions
            {
                senedrId = modelreq.sender,
                receiverId = Guid.Parse(container),
                amount = modelreq.amount,
                createdAt = DateTime.Now,
                status= "Success"
            };
            transRepo.Add(trans);
            transRepo.Save();
            res.Message = "Success";
            res.Data = trans;
            return StatusCode(StatusCodes.Status200OK, res);
        }
        [HttpGet("{UserId}")]
        public async Task<IActionResult> getAll(Guid UserId)
        {
            List<dynamic> temp = new List<dynamic>();
            var transactions = await transRepo.getTransaction(UserId);
            if (transactions == null)
            {
                res.Message = "Failed";
                res.Data = "No transactions found";
                return StatusCode(StatusCodes.Status404NotFound, res);
            }
            foreach (var item in transactions)
            {
                
                if (item.senedrId == UserId)
                {
                    var obj = new
                    {
                        item.Id,
                        item.senedrId,
                        item.receiverId,
                        item.amount,
                        item.createdAt,
                        Type = "Send"
                    };
                    temp.Add(obj);
                }
                else
                {
                    var obj = new
                    {
                        item.Id,
                        item.senedrId,
                        item.receiverId,
                        item.amount,
                        item.createdAt,
                        Type = "Receive"
                    };
                temp.Add(obj);
                }
            }
                res.Message = "Success";
                res.Data = temp;
                return StatusCode(StatusCodes.Status200OK, res);
            }
            [HttpGet("getByid/{id:int}")]
            public async Task<IActionResult> getById(int id)
            {
                var transaction = await transRepo.getById(id);
                if (transaction == null)
                {
                    res.Message = "Failed";
                    res.Data = "Transaction not found";
                    return StatusCode(StatusCodes.Status404NotFound, res);
                }
                res.Message = "Success";
                res.Data = transaction;
                return StatusCode(StatusCodes.Status200OK, res);
            }
        }
    }
