using System.Collections.Generic;

namespace IcampusBoatBackend.Models.Settings
{
    public class StudentDataUpload
    {
        public string? IDENT { get; set; }
        public string? ADMISSIONNO { get; set; }
        public string? SNAME { get; set; }
        public string? ACADEMICYEAR { get; set; }
        public string? columns { get; set; }
        public string? columnName { get; set; }
        public string? columnText { get; set; }
        public List<Dictionary<string, object>>? StudentData { get; set; }
    }
}
