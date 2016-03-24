using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace site.Models
{
    public class Answer
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public virtual Sentence Task { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string AnswerText { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public bool IsWarning { get; set; }
    }
}