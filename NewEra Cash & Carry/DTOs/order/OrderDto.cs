namespace NewEra_Cash___Carry.DTOs.order
{
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
            public List<OrderItemDto> OrderItems { get; set; }
        }
    }

}
