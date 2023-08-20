using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedSheetApp1.Models
{
	public class Keywords
	{
		public int KeywordsID { get; set; }
		public int QuestionID { get; set; }
		[DataType(DataType.Date)]
		public DateTime CreateDate { get; set; }
		public string Word { get; set; }
		public int Position { get; set; }
		public bool? RightOrWrong { get; set; }
	}
}
