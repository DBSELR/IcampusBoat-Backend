using System;
using System.Collections.Generic;

namespace IcampusBoatBackend.Models.Admissions
{
    public class ExpenditureSaveRequest
    {
        public string RegNo { get; set; } = string.Empty; // Registration No / TxtAdmino
        public string StudentName { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public List<ExpenditureHeadItem> Heads { get; set; } = new List<ExpenditureHeadItem>();
    }

    public class ExpenditureHeadItem
    {
        public string Year { get; set; } = string.Empty;
        public string ExpenditureHeads { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
