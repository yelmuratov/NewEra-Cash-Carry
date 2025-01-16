namespace NewEra_Cash___Carry.DTOs.order
{
    using global::NewEra_Cash___Carry.Core.DTOs.order;
    using System;
    using System.Collections.Generic;

    namespace NewEra_Cash___Carry.DTOs.order
    {
        public class OrderDto
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public string UserName { get; set; }
            public DateTime OrderDate { get; set; }
            public decimal TotalAmount { get; set; }
            public string Status { get; set; }
            public string PaymentStatus { get; set; }
            public string? PaymentIntentId { get; set; } // Include Payment Intent ID if needed
            public List<OrderItemDto> OrderItems { get; set; }
        }
    }
}
