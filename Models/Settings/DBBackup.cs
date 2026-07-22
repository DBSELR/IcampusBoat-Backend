namespace IcampusBoatBackend.Models.Settings
{
    public class DBBackup
    {
        public string? Path { get; set; }
        public string? SpName { get; set; }
        public string? Date { get; set; }
        public string Go { get; set; } = "GO";
        public string ANSI_NULLS { get; set; } = "SET ANSI_NULLS ON";
        public string QUOTED_IDENTIFIER { get; set; } = "SET QUOTED_IDENTIFIER ON";
        public string Separator { get; set; } = "/****** Object:  StoredProcedure   ******/";
        public string FromMailid { get; set; } = "dbasebackups2011@gmail.com";
        public string MailPswd { get; set; } = "dbase@2011";
        public string ToMailid { get; set; } = "dbasebackups2011@gmail.com";
        public string MailSubjectPrefixName { get; set; } = "LBRCE-ERP-SP-SCRIPT_";
        public string MailBody { get; set; } = "Mail From LBRC-ENGG-ERP with attachment";
        public string ClientbackUpPrefixName { get; set; } = "LBRCE-ERP-DB-";
        public string? ClientbackUpFileName { get; set; }
        public string? EmpId { get; set; }
        public string? TakenDate { get; set; }
    }
}
