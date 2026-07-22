namespace IcampusBoatBackend.Models.Settings
{
    public class ProgrammeMaster
    {
        public string? CID { get; set; }
        public string? COURSECODE { get; set; }
        public string? COURSE { get; set; }
        public string? DEGREE { get; set; }
        public string? YEAR { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public string? FinancialYear { get; set; }
    }
}
