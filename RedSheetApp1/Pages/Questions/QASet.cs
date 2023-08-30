using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RedSheetApp1.Pages.Questions
{
	public class QASet
	{
		public string Question { get; set; }
		public string Answer { get; set; }
	}
}
