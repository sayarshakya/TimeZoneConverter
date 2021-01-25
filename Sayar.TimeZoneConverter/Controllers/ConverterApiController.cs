using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sayar.TimeZoneConverter.Data;
using Sayar.TimeZoneConverter.Models;

namespace Sayar.TimeZoneConverter.Controllers
{
 
    [Route("api/[controller]")]
    [ApiController]
    public class ConverterApiController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ConverterApiController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        [Route("GetTimeZone")]
        public List<SelectListItem> GetTimeZone()
        {
            var myTimeZoneList = new List<SelectListItem>();
            foreach (var timeZoneInfo in TimeZoneInfo.GetSystemTimeZones())
            {
                myTimeZoneList.Add(new SelectListItem { Text = timeZoneInfo.DisplayName, Value = timeZoneInfo.Id });
            }
            return myTimeZoneList;
        }

        [HttpPost]
        [Route("ConvertRequest")]
        public string ConvertRequest([FromBody] ConversionViewModel model)
        {
           
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
            var getLimitedTimeZone = TimeZoneInfo.GetSystemTimeZones().Where(x => x.BaseUtcOffset.Hours <= -4 && x.BaseUtcOffset.Hours >= -9).ToList();
            bool check = getLimitedTimeZone.Any(x => x.BaseUtcOffset.Hours == timeZoneHours);
            if (role != null)
            {
                if(role == Enums.Roles.User.ToString() && check)
                {
                   return $"You don't have access to convert {getTimeZoneDetail.DisplayName.ToString()} Timezone.";
                }
            }
            
            DateTime dateToConvert = Convert.ToDateTime(model.MyDate).Add(TimeSpan.Parse(model.Time));
            DateTime convertedDate = TimeZoneInfo.ConvertTime(dateToConvert, TimeZoneInfo.FindSystemTimeZoneById(model.ConvertTo));
            return $"Time and date in {getTimeZoneDetail.DisplayName.ToString()} is {string.Format("{0:G}", convertedDate)}";
        }
    }
}