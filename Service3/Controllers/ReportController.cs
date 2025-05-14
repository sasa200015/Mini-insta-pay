using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service3.DTO;
using Service3.Interface;
using Service3.Model;

namespace Service3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReport reportRepo;
        private readonly generalRes res;

        public ReportController(IReport reportRepo, generalRes res)
        {
            this.reportRepo = reportRepo;
            this.res = res;
        }
        [HttpPost]
        public async Task<IActionResult> addReport([FromBody] Add modelreq)
        {
            try
            {
                var report = new Report
                {
                    SenderId = modelreq.SenderId,
                    ReceiverId = modelreq.ReceiverId,
                    transactionType = modelreq.TransactionType,
                    amount = modelreq.Amount,
                    transactionDate = modelreq.TransactionDate
                };
                await reportRepo.addReport(report);
                await reportRepo.Saveasync();
                res.Message = "Success";
                res.Data = modelreq;
                return StatusCode(StatusCodes.Status200OK, res);
            }
            catch (Exception ex)
            {
                res.Message = "Failed";
                res.Data = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, res);
            }
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetHistory(Guid userId)
        {
            try
            {
                var result = await reportRepo.GetHistory(userId);
                foreach (var item in result)
                {
                    if (userId == item.SenderId)
                    {
                        item.transactionType = "Send";
                    }
                    else
                    {
                        item.transactionType = "Receive";
                    }
                }
                    res.Message = "Success";
                res.Data = result;
                return StatusCode(StatusCodes.Status200OK, res);
            }
            catch (Exception ex)
            {
                res.Message = "Failed";
                res.Data = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, res);
            }
        }
        [HttpGet("Summary/{userId}")]
        public async Task<IActionResult> GetSummary(Guid userId)
        {
            try
            {
                decimal totalSent = await reportRepo.totalSent(userId);
                decimal totalReceive = await reportRepo.totalReceive(userId);
                res.Message = "Success";
                res.Data = new { totalSent, totalReceive };
                return StatusCode(StatusCodes.Status200OK, res);
            }
            catch (Exception ex)
            {
                res.Message = "Failed";
                res.Data = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, res);
            }
        }
    }
}
