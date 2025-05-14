using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Service1.DTO;
using Service1.Model;

namespace Service1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<Users> usermanager;
        private readonly generalRes res;
        private readonly IConfiguration config;
        private readonly Project_Context context;

        public UserController(UserManager<Users> usermanager, generalRes res, IConfiguration config, Project_Context context)
        {
            this.usermanager = usermanager;
            this.res = res;
            this.config = config;
            this.context = context;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> register([FromForm] register modelreq)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var exist = await usermanager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == modelreq.Phone_Number);
                    if(exist != null)
                    {
                        res.message = "Failed";
                        res.data = "Phone number already exists";
                        return StatusCode(StatusCodes.Status400BadRequest, res);
                    }
                    Users user = new Users()
                    {
                        UserName = modelreq.User_Name,
                        Email = modelreq.Email,
                        PhoneNumber = modelreq.Phone_Number,
                        createdAt = DateTime.Now,
                        balance = 0
                    };

                    // Fixing the error by passing the correct type (user object) to GetPhoneNumberAsync                  
                    IdentityResult result = await usermanager.CreateAsync(user, modelreq.Password);
                    if (result.Succeeded)
                    {
                        res.message = "Success";
                        res.data = new
                        {
                            user.Email,
                            user.UserName,
                            user.PhoneNumber,
                            user.PasswordHash,
                            user.balance,
                            user.createdAt
                        };
                        return StatusCode(StatusCodes.Status200OK, res);
                    }
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("Error", item.Description);
                    }
                }
                catch (Exception ex)
                {
                    res.message = "unexpected error";
                    res.data = ex.Message;
                    return StatusCode(StatusCodes.Status500InternalServerError, res);
                }
            }
            var modelStateErrors = ModelState
         .Where(ms => ms.Value.Errors.Any()) // Only include entries with errors
         .ToDictionary(
             ms => ms.Key, // Keep the key (field name)
             ms => ms.Value.Errors.Select(e => e.ErrorMessage).ToList() // Only include error messages
          );
            res.message = "Failed";
            res.data = modelStateErrors;
            return StatusCode(StatusCodes.Status400BadRequest, res);
        }
        [HttpPost("login")]
        public async Task<IActionResult> login([FromForm] login modelreq)
        {
            if (ModelState.IsValid)
            {
                Users user = await usermanager.FindByEmailAsync(modelreq.Email);
                if (user != null)
                {
                    bool found = await usermanager.CheckPasswordAsync(user, modelreq.Password);
                    if (found)
                    {

                        List<Claim> UserClaims = new List<Claim>();
                        UserClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                        UserClaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                        UserClaims.Add(new Claim(ClaimTypes.Name, user.UserName));
                        var userRole = await usermanager.GetRolesAsync(user);

                        SymmetricSecurityKey key = new SymmetricSecurityKey(
                            Encoding.UTF8.
                            GetBytes(config["JWT:SecritKey"]));

                        SigningCredentials credentials = new SigningCredentials(
                            key,
                            SecurityAlgorithms.HmacSha256);

                        JwtSecurityToken token = new JwtSecurityToken(
                            issuer: config["JWT:IssuerIP"],
                            audience: config["JWT:AudienceIP"],
                            expires: DateTime.Now.AddMinutes(20),
                            claims: UserClaims,
                            signingCredentials: credentials

                            );
                        //split the Date and time to HH:mm:ss
                        Token generated = new Token();
                        DateTime spliter = DateTime.Now.AddMinutes(20);
                        string dateTimeString = spliter.ToString("yyyy-MM-ddTHH:mm:ss");
                        string[] splitResult = dateTimeString.Split('T');
                        generated.token = new JwtSecurityTokenHandler().WriteToken(token);
                        generated.expires = splitResult[1];
                        res.message = "Success";
                        res.data = generated;

                        return StatusCode(StatusCodes.Status200OK, res);
                    }
                }
                ModelState.AddModelError("UserName", "UserName or Password invalid");
                res.message = "Failed";
                res.data = ModelState["UserName"].Errors[0].ErrorMessage;
                return StatusCode(StatusCodes.Status400BadRequest, res);
            }
            res.message = "Failed";
            res.data = ModelState;
            return StatusCode(StatusCodes.Status400BadRequest, res);
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> getId(string id)
        {
            var user = await usermanager.FindByIdAsync(id);
            if (user == null)
            {
                res.message = "Failed";
                res.data = "User not found";
                return StatusCode(StatusCodes.Status404NotFound, res);
            }
            res.message = "Success";
            res.data = new
            {
                user.Email,
                user.UserName,
                user.PhoneNumber,
                user.balance,
                user.createdAt
            };
            return StatusCode(StatusCodes.Status200OK, res);
        }
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> updateBalance(string id, balance reqmodel)
        {
            var user = await usermanager.FindByIdAsync(id);
            if (user == null)
            {
                res.message = "Failed";
                res.data = "User not found";
                return StatusCode(StatusCodes.Status404NotFound, res);
            }

            user.balance += reqmodel.amount;
            if (user.balance < 0)
            {
                res.message = "Failed";
                res.data = "Insufficient balance";
                return StatusCode(StatusCodes.Status400BadRequest, res);
            }
            var result = await usermanager.UpdateAsync(user);
            if (result.Succeeded)
            {
                res.message = "Success";
                res.data = new
                {
                    user.Email,
                    user.UserName,
                    user.PhoneNumber,
                    newbalance = user.balance,
                };
                return StatusCode(StatusCodes.Status200OK, res);
            }
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("Error", item.Description);
            }
            res.message = "Failed";
            res.data = ModelState;
            return StatusCode(StatusCodes.Status400BadRequest, res);
        }
        [HttpGet("{phone}")]
        public async Task<IActionResult> getPhone(string phone)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phone);
            if (user == null)
            {
                res.message = "Failed";
                res.data = "User not found";
                return StatusCode(StatusCodes.Status404NotFound, res);
            }
            res.message = "Success";
            res.data = new
            {
                user.Id,
                user.Email,
                user.UserName,
                user.PhoneNumber,
                user.balance,
                user.createdAt
            };
            return StatusCode(StatusCodes.Status200OK, res);
        }
    }
}
