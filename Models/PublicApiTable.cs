using System.Text.Json.Serialization;

namespace Swilago.Models
{
    /// <summary>
    /// 프론트에 로그인 한 접속자의 Email을 DB에서 검색하고
    /// 프로시저로 가공된 가장 최근 접속기록의 Row 데이터를 가져와서 여기에 넣고
    /// JSON으로 만들어 프론트로 보냄
    /// </summary>
    public class PublicApiTable
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public bool IsSelected { get; set; }

        public string? Info { get; set; }
    }
}
