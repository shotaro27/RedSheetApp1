using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedSheetApp1.Models
{
	public class Keyword
	{
		public int Id { get; set; }
		[DataType(DataType.Date)]
		public DateTime CreateDate { get; set; }
	}
}
