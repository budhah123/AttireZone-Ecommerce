using System;

namespace AttireZone_Web_App.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; }

        public string UserFullName { get; set; }
    }
}
