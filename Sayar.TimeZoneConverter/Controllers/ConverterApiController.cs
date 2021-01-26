using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Sayar.TimeZoneConverter.Data;
using Sayar.TimeZoneConverter.Models;

namespace Sayar.TimeZoneConverter.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ConverterApiController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public ConverterApiController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GetTimeZone")]
        public List<SelectListItem> GetTimeZone()
        {
            try
            {
                var myTimeZoneList = new List<SelectListItem>();
                foreach (var timeZoneInfo in TimeZoneInfo.GetSystemTimeZones())
                {
                    myTimeZoneList.Add(new SelectListItem { Text = timeZoneInfo.DisplayName, Value = timeZoneInfo.Id });
                }
                return myTimeZoneList.OrderBy(x => x.Text).ToList();
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        [HttpPost]
        [Route("ConvertRequest")]
        public IActionResult ConvertRequest([FromBody] ConversionViewModel model)
        {
            //if(!ModelState.IsValid)
            //{
            //    return Ok("Validation Failed");
            //}

            try
            {
                int StartRange = _configuration.GetValue<int>("AccessDenied:StartRange");
                int EndRange = _configuration.GetValue<int>("AccessDenied:EndRange");

                var getTimeZoneDetail = TimeZoneInfo.FindSystemTimeZoneById(model.ConvertTo);
                var userId = _userManager.GetUserId(User);
                var role = (from acc in _context.Users
                            join userRoles in _context.UserRoles on acc.Id equals userRoles.UserId into accuserRoles
                            from userRoles in accuserRoles.DefaultIfEmpty()
                            join rol in _context.Roles on userRoles.RoleId equals rol.Id into userRolesrol
                            from rol in userRolesrol.DefaultIfEmpty()
                            where acc.Id == userId
                            select rol.Name).FirstOrDefault();
                int timeZoneHours = getTimeZoneDetail.BaseUtcOffset.Hours;
                var getLimitedTimeZone = TimeZoneInfo.GetSystemTimeZones().Where(x => x.BaseUtcOffset.Hours <= StartRange && x.BaseUtcOffset.Hours >= EndRange).ToList();
                bool check = getLimitedTimeZone.Any(x => x.BaseUtcOffset.Hours == timeZoneHours);
                if (role != null)
                {
                    if (role == Enums.Roles.User.ToString() && check)
                    {
                        return Ok($"You don't have access to convert {getTimeZoneDetail.DisplayName.ToString()} Timezone.");
                    }
                }

                DateTime dateToConvert = Convert.ToDateTime(model.MyDate).Add(TimeSpan.Parse(model.Time));
                DateTime convertedDate = TimeZoneInfo.ConvertTime(dateToConvert, TimeZoneInfo.FindSystemTimeZoneById(model.ConvertTo));
                return Ok($"Time and date in {getTimeZoneDetail.DisplayName.ToString()} is {convertedDate.ToString("MM/dd/yyyy HH:mm:ss")}");
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }
    }
}