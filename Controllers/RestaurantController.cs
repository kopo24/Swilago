using Borago.Models;
using Microsoft.AspNetCore.Mvc;
using Swilago.Interfaces;
using Swilago.Models;
using System.Text;

namespace Swilago.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _service;

        public RestaurantController(IRestaurantService service)
        {
            _service = service;
        }

        // 접속자 최근 선택항목 가져오기: /Restaurant/GetResList?userEmail=
        [HttpGet]
        [Route("GetResList")]
        public IActionResult GetRestaurantList(string? userEmail)
        {
            return CheckErrors(_service.GetRestaurantList(userEmail));
        }

        // 룰렛 정보 저장: /Restaurant/PostRouletteInfo?userEmail=&rouletteResult=
        // ex) selResRecord = [{ text: "text" }, ..., { text: "text" }] is in Body
        [HttpPost]
        [Route("PostRouletteInfo")]
        public IActionResult PostRouletteInfo(string? userEmail, string? rouletteResult, [FromBody] List<SelResRecord>? selResRecords)
        {
            return _service.PostRouletteInfo(userEmail, rouletteResult, selResRecords);
        }

        // 전체 통계 가져오기: /Restaurant/GetStat
        [HttpGet]
        [Route("GetStat")]
        public IActionResult GetStatistics()
        {
            return CheckErrors(_service.GetStatistics());
        }

        // 월, 주 별로 통계 가져오기: /Restaurant/GetDateStat?year=&month=&week=
        [HttpGet]
        [Route("GetDateStat")]
        public IActionResult GetDateStatistics(int year, int month, int week)
        {
            return CheckErrors(_service.GetDateStatistics(year, month, week));
        }

        //---------------------------------------------------------------------------

        // 식당 이름 검색: /Restaurant/GetRes?resName=
        [HttpGet]
        [Route("GetRes")]
        public IActionResult GetRestaurant(string? resName)
        {
            return CheckErrors(_service.GetRestaurant(resName));
        }

        // 식당 추가: /Restaurant/PostRes?resName=&resInfo=
        [HttpPost]
        [Route("PostRes")]
        public IActionResult PostRestaurant(string? resName, string? resInfo)
        {
            return _service.PostRestaurant(resName, resInfo);
        }

        // 식당 삭제: /Restaurant/DeleteRes?resName=
        [HttpDelete]
        [Route("DeleteRes")]
        public IActionResult DeleteRes(string? resName)
        {
            return _service.DeleteRestaurnat(resName);
        }

        // 식당 정보 수정: /Restaurant/PutResInfo?resName=&resInfo=
        // 식당 이름 수정은 PK값이 식당 이름이라 불가능. 이름수정은 삭제 후 다시 추가해야 함.
        [HttpPut]
        [Route("PutResInfo")]
        public IActionResult PutResInfo(string? resName, string? resInfo)
        {
            return _service.PutRestaurnat(resName, resInfo);
        }

        public IActionResult CheckErrors(Payload payload)
        {
            if (payload == null)
                return Ok();

            if (payload.ErrorMessages.Count > 0)
                return StatusCode((int)payload.StatusCode,
                                  string.Join(Environment.NewLine, payload.ErrorMessages.ToArray()));

            return Content(payload.Message, "application/json", Encoding.UTF8);
        }
    }
}
