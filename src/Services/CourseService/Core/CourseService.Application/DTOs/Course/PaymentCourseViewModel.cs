﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.DTOs.Course
{
    public class PaymentCourseViewModel
    {
        public Guid CourseId { get; set; }
        public string CardNumber { get; set; }
        public string expiryDate { get; set; }
        public string CVV { get; set; }
    }
}
