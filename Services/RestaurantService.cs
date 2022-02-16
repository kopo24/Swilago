using Borago.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Swilago.Data;
using Swilago.Data.Procedures;
using Swilago.Data.Tables;
using Swilago.Interfaces;
using Swilago.Models;
using System.Diagnostics;
using System.Net;
using System.Reflection;

namespace Swilago.Services
{
    public class RestaurantService : ControllerBase, IRestaurantService
    {
        private readonly IConfiguration _config;
        private readonly PamukkaleContext _context;

        public RestaurantService(IConfiguration config, PamukkaleContext context)
        {
            _config = config;
            _context = context;
        }

        // 접속자 최근 선택항목 가져오기
        public Payload GetRestaurantList(string? userEmail)
        {
            Payload payload = new();

            try
            {
                if (!userEmail.Contains('@'))
                    userEmail = "";

                // userEmail을 파라메터에 넣어서 프로시저 실행
                SqlParameter paramUserEmail = new("@UserEmail", userEmail);

                var publicApiTable = _context.Set<PPublicApi>()
                                             .FromSqlRaw(MyQueries.PublicApi, paramUserEmail)
                                             .AsNoTracking();

                List<PublicApiTable> publicApiRowList = new();

                bool rowSelected;
                foreach (var publicApiRow in publicApiTable)
                {
                    rowSelected = (publicApiRow.IsSelected.ToString().Equals("true"));
                    publicApiRowList.Add(new PublicApiTable()
                    {
                        Id = publicApiRow.Id,
                        Text = publicApiRow.Text,
                        IsSelected = rowSelected,
                        Info = publicApiRow.Info
                    });
                }

                payload.Message = JsonConvert.SerializeObject(publicApiRowList, Formatting.Indented);
            }
            catch (Exception ex)
            {
                SetErrorMessages(ref payload, ex);
            }

            return payload;
        }

        // 룰렛 정보 저장
        public IActionResult PostRouletteInfo(string? userEmail, string? rouletteResult, [FromBody] List<SelResRecord> selResRecords)
        {
            if (!userEmail.Contains('@') || rouletteResult == null || rouletteResult == "" || selResRecords.Count() < 1)
                return BadRequest();

            string resRecord = "";
            foreach (var selResRecord in selResRecords)
                resRecord += selResRecord.Text.ToString() + ",";

            TUserRecord userRecord = new()
            {
                ModifiedDate = DateTime.Now,
                RouletteResult = rouletteResult,
                UserEmail = userEmail,
                ResRecord = resRecord
            };

            _context.UserRecord.Add(userRecord);
            _context.SaveChanges();

            return NoContent();
        }

        // 전체 통계 가져오기
        public Payload GetStatistics()
        {
            Payload payload = new();

            try
            {
                var allStatisticsTable = _context.Set<PAllStatistics>()
                                                 .FromSqlRaw(MyQueries.AllStatistics)
                                                 .AsNoTracking()
                                                 .ToList();

                int addRank = 1;
                foreach (var row in allStatisticsTable)
                    row.ResRank = addRank++;

                payload.Message = JsonConvert.SerializeObject(allStatisticsTable, Formatting.Indented);
            }
            catch (Exception ex)
            {
                SetErrorMessages(ref payload, ex);
            }

            return payload;
        }

        // 월, 주 별로 통계 가져오기
        public Payload GetDateStatistics(int year, int month, int week)
        {
            Payload payload = new();

            try
            {
                // DaysInMonth의 예외 처리, 올 해의 년도가 기본으로.
                if (year < 1 || year > 9999)
                    year = DateTime.Now.Year;

                // DaysInMonth의 예외 처리, 현재 월을 기본으로.
                if (month < 1 || month > 12)
                    month = DateTime.Now.Month;

                string refdate = "월";
                // Range로 입력받은 년월의 일자를 나열하고, Where로 refdate요일의 일자만 조건주고, Select로 해당 날짜를 ToList형식으로 가져오기.
                var weeks = Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                                        .Where(d => new DateTime(year, month, d).ToString("ddd").Equals(refdate))
                                        .Select(d => new DateTime(year, month, d)).ToList();

                // 기본으로 이번 달 통계 넘기기
                DateTime startDate = new(year, month, 1);
                DateTime endDate = new(year, month, DateTime.DaysInMonth(year, month));

                // 월(month)에 입력한 주차(week)가 없으면 (ex> week = -1 || 0 || 14) 이번 달 통계를 넘김
                if (0 < week && week <= weeks.Count)
                {
                    startDate = weeks[week - 1];
                    endDate = weeks[week - 1].AddDays(6);
                }

                SqlParameter paramStartDate = new("@StartDate", startDate.ToString("yyyy-MM-dd"));
                SqlParameter paramEndDate = new("@EndDate", endDate.ToString("yyyy-MM-dd"));

                var dateStatisticsTable = _context.Set<PDateStatistics>()
                                                  .FromSqlRaw(MyQueries.DateStatistics, paramStartDate, paramEndDate)
                                                  .AsNoTracking()
                                                  .ToList();

                int addRank = 1;
                foreach (var row in dateStatisticsTable)
                    row.ResRank = addRank++;

                payload.Message = JsonConvert.SerializeObject(dateStatisticsTable, Formatting.Indented);
            }
            catch (Exception ex)
            {
                SetErrorMessages(ref payload, ex);
            }

            return payload;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------

        // 식당 이름 검색
        public Payload GetRestaurant(string? resName)
        {
            Payload payload = new();

            try
            {
                var resRows = _context.Restaurant.AsNoTracking()
                                                 .Where(r => r.ResName.Contains(resName));
                List<Restaurant> foundResList = new();

                foreach (var resRow in resRows)
                {
                    foundResList.Add(new Restaurant()
                    {
                        ResName = resRow.ResName,
                        ResInfo = resRow.ResInfo
                    });
                }

                payload.Message = JsonConvert.SerializeObject(foundResList, Formatting.Indented);
                
                if (resName == "null" || resName == null || resName == "")
                {
                    var resAllRows = _context.Restaurant.AsNoTracking().ToList();
                    payload.Message = JsonConvert.SerializeObject(resAllRows, Formatting.Indented);
                }
                    
            }
            catch (Exception ex)
            {
                SetErrorMessages(ref payload, ex);
            }

            return payload;
        }

        // 식당 추가
        public IActionResult PostRestaurant(string? resName, string? resInfo)
        {
            if (resName == "null" || resName == null || resName == "")
                return BadRequest();

            TRestaurant newResRow = new()
            {
                ResName = resName,
                ResInfo = resInfo
            };

            _context.Restaurant.Add(newResRow);
            _context.SaveChanges();

            return NoContent();
        }

        // 식당 삭제
        public IActionResult DeleteRestaurnat(string? resName)
        {
            if (resName == "null" || resName == null || resName == "")
                return BadRequest();

            var existsRow = _context.Restaurant.AsNoTracking()
                                               .Where(r => r.ResName == resName)
                                               .SingleOrDefault();

            if (existsRow == null)
                return NotFound();

            _context.Restaurant.Remove(existsRow);
            _context.SaveChanges();

            return NoContent();
        }

        // 식당 정보 수정(식당 이름은 PK, 고유값으로 수정이 안되고 삭제 후 다시 추가로만 변경해야 함)
        public IActionResult PutRestaurnat(string? resName, string? resInfo)
        {
            if (resName == "null" || resName == null || resName == "")
                return BadRequest();

            var existsRow = _context.Restaurant.AsNoTracking()
                                               .Where(r => r.ResName == resName)
                                               .SingleOrDefault();

            if (existsRow == null)
                return NotFound();

            existsRow.ResInfo = resInfo;

            if (resName != existsRow.ResName.ToString())
                return BadRequest();

            _context.Restaurant.Update(existsRow);
            _context.SaveChanges();

            return NoContent();
        }
        
        public void SetErrorMessages(ref Payload payload, Exception ex)
        {
            var trace = new StackTrace(ex);
            var assembly = Assembly.GetExecutingAssembly();
            var methodName = trace.GetFrames().Select(f => f.GetMethod()).First(m => m.Module.Assembly == assembly).Name;

            payload.ErrorMessages.Add(new string('-', 10));
            payload.ErrorMessages.Add($"## Exception from {methodName}");
            payload.ErrorMessages.Add($"## Exception Message {ex.Message}");

            
            if (ex.InnerException != null)
                payload.ErrorMessages.Add($"## Inner Exception Message {ex.InnerException.Message}");

            payload.ErrorMessages.Add(new string('-', 10));

            if (ex is WebException)
            {
                HttpWebResponse response = (HttpWebResponse)(ex as WebException).Response;
                payload.StatusCode = response.StatusCode;
            }

            Debug.WriteLine(string.Join(Environment.NewLine, payload.ErrorMessages.ToArray()));
        }
    }
}
