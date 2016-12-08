using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace TodoApi.Models
{
    public class NotSerializable
	{
		public int Prop1 { get; set; }
		public int Prop2 { get; set; }

		public int Prop3
		{
			get
			{
				throw new Exception("Not serialisable");
			}
		}
	}
}

