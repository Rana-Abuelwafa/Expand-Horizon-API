using ITravel_App.services;
using ITravel_App.Services;
using ITravelApp.Data.Entities;
using ITravelApp.Data.Models.Bookings;
using ITravelApp.Data.Models.global;
using ITravelApp.Data.Models.profile;
using ITravelApp.Data.Models.trips;
using Mails_App;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace ITravel_App.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : Controller
    {
        IMailService Mail_Service = null;
        private readonly CustomViewRendererService _viewService;
        private readonly IClientService _clientService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LoginUserData _loginUserData;
        public BookingController(IMailService _MailService, CustomViewRendererService viewService, IHttpContextAccessor httpContextAccessor, IClientService clientService)
        {
            _viewService = viewService;
            _clientService = clientService;
            _httpContextAccessor = httpContextAccessor;
            _loginUserData = Utils.getTokenData(httpContextAccessor);
            Mail_Service = _MailService;
        }

        #region reviews
       
        [HttpPost("SaveReviewForTrip")]
        public IActionResult SaveReviewForTrip(tbl_review row)
        {
            string? clientId = _loginUserData.client_id;
            row.client_id=clientId;
            return Ok(_clientService.SaveReviewForTrip(row));
        }
        #endregion

        #region "wishlist"

        [HttpPost("AddTripToWishList")]
        public IActionResult AddTripToWishList(TripsWishlistReq row)
        {
            string? clientId = _loginUserData.client_id;
            row.client_id = clientId;
            return Ok(_clientService.AddTripToWishList(row));
        }


        [HttpPost("GetClientWishList")]
        public async Task<IActionResult> GetClientWishList(ClientWishListReq req)
        {
            string? clientId = _loginUserData.client_id;
            req.client_id = clientId;
            return Ok(await _clientService.GetClientWishList(req));
        }

        
        #endregion


        #region "Booking"
        [HttpPost("SaveClientBooking")]
        public IActionResult SaveClientBooking(BookingCls row)
        {

            string? clientId = _loginUserData.client_id;
            string? email = _loginUserData.client_email;
           
            row.client_id = clientId;
            row.client_email = email;
            return Ok(_clientService.SaveClientBooking(row));
        }
        [HttpPost("GetTrip_Extra_Mains")]
        public IActionResult GetTrip_Extra_Mains(TripExtraReq req)
        {
            return Ok( _clientService.getFacilityForTrip(req.trip_id,req.lang_code,req.isExtra));
            //return Ok(await _clientService.GetTrip_Extra_Mains(req));
        }
        [HttpPost("AssignExtraToBooking")]
        public IActionResult AssignExtraToBooking(List<booking_extra> lst)
        {
            return Ok( _clientService.AssignExtraToBooking(lst));
        }
        [HttpPost("CalculateBookingPrice")]
        public IActionResult CalculateBookingPrice(CalculateBookingPriceReq req)
        {
            return Ok(_clientService.CalculateBookingPrice(req));
        }

        [HttpPost("GetBookingSummary")]
        public async Task<IActionResult> GetBookingWithDetails(BookingReq req)
        {
            string? clientId = _loginUserData.client_id;
            req.client_id = clientId;
            return Ok(await _clientService.GetBookingWithDetails(req));
        }

        [HttpPost("ConfirmBooking")]
        public async Task<IActionResult> ConfirmBooking(ConfirmBookingReq req)
        {
            string? clientId = _loginUserData.client_id;
            string? client_email = _loginUserData.client_email;
            string? FullName = _loginUserData.FullName;
            req.client_id = clientId;
            string fileName = "BookingClientEmail_" + req.lang_code.ToLower() + ".cshtml";
            var templatePath = Path.Combine("/Views/Email" + "/", fileName);
            BookingWithTripDetailsAll model = _clientService.ConfirmBooking(req);
            if (model != null)
            {
                model.client_name = FullName;
                var msg = await _viewService.RenderViewToStringAsync(templatePath, model, ControllerContext);
                MailData Mail_Data = new MailData { EmailToId = client_email, EmailToName = FullName, EmailSubject = UtilsCls.GetMailSubjectByLang(req.lang_code, 3), EmailBody = msg };
                return Ok(Mail_Service.SendMail(Mail_Data));
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    model);
                //return BadRequest(model);
            }
            
        }
        #endregion


        #region "Profile"
        [HttpPost("GetClientProfiles")]
        public async Task<IActionResult> GetClientProfiles()
        {
            string? clientId = _loginUserData.client_id;
            // string? email = _loginUserData.client_email;
            return Ok(await _clientService.GetClientProfiles(clientId));
        }

        [HttpPost("GetProfileImage")]
        public async Task<IActionResult> GetProfileImage()
        {
            string? clientId = _loginUserData.client_id;
            // string? email = _loginUserData.client_email;
            return Ok(await _clientService.GetProfileImage(clientId));
        }
        [HttpPost("saveProfileImage")]
        public async Task<IActionResult> SaveProfileImage(ImgCls cls)
        {
            string? clientId = _loginUserData.client_id;
            var path = Path.Combine("images" + "//", cls.img.FileName);
            //var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Images" + "//", cls.img.FileName);
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                cls.img.CopyTo(stream);
                stream.Close();
            }



            client_image image = new client_image
            {
                client_id = clientId,
                img_name = cls.img.FileName,
                img_path = path,
                type = 1  //mean save profile image
            };

            return Ok(await _clientService.SaveProfileImage(image));
        }
        [HttpPost("SaveMainProfile")]
        public IActionResult SaveMainProfile(ClientProfileCast profile)
        {
            string? clientId = _loginUserData.client_id;
            string? email = _loginUserData.client_email;
            string? FullName = _loginUserData.FullName;
            profile.client_id = clientId;
            profile.client_name = FullName;
            profile.client_email = email;
            return Ok(_clientService.SaveMainProfile(profile));
        }
        [HttpPost("GetClient_Notification_Settings")]
        public async Task<IActionResult> GetClient_Notification_Settings()
        {
            string? clientId = _loginUserData.client_id;
            return Ok(await _clientService.GetClient_Notification_Settings(clientId));
        }
        [HttpPost("SaveClientNotificationSetting")]
        public IActionResult SaveClientNotificationSetting(client_notification_setting row)
        {
            string? clientId = _loginUserData.client_id;
            string? email = _loginUserData.client_email;
            string? FullName = _loginUserData.FullName;
            row.client_id = clientId;
            return Ok(_clientService.SaveClientNotificationSetting(row));
        }
        #endregion
    }
}
