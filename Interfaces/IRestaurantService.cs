using Borago.Models;
using Microsoft.AspNetCore.Mvc;
using Swilago.Models;

namespace Swilago.Interfaces
{
    public interface IRestaurantService
    {
        // 접속자 최근 선택항목 가져오기
        Payload GetRestaurantList(string? userEmail);

        // 룰렛 정보 저장
        IActionResult PostRouletteInfo(string? userEmail, string? rouletteResult, [FromBody] List<SelResRecord> selResRecords);

        // 전체 통계 가져오기
        Payload GetStatistics();

        // 월, 주 별로 통계 가져오기
        Payload GetDateStatistics(int year, int month, int week);

        //---------------------------------------------------------------------------

        // 식당 이름 검색
        Payload GetRestaurant(string? resName);

        // 식당 추가
        IActionResult PostRestaurant(string? resName, string? resInfo);

        // 식당 삭제
        IActionResult DeleteRestaurnat(string? resName);
        
        // 식당 정보 수정(식당 이름은 PK, 고유값으로 수정이 안되고 삭제 후 다시 추가로만 변경해야 함)
        IActionResult PutRestaurnat(string? resName, string? resInfo);
    }
}
