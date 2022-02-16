namespace Swilago.Models
{
    /// <summary>
    /// 프론트에서 룰렛에 들어간 식당 이름들을 JSON으로 보내고
    /// [FromBody] List<SelResRecord>로 받음
    /// </summary>
    public class SelResRecord
    {
        public string Text { get; set; }
    }
}
