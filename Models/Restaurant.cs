namespace Swilago.Models
{
    /// <summary>
    /// 식당 이름 검색할 때 DB의 검색 결과를
    /// 여기에 담아서 JSON으로 바꾸고 프론트로 보냄
    /// </summary>
    public class Restaurant
    {
        public string ResName { get; set; }

        public string? ResInfo { get; set; }
    }
}
