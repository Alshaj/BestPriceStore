namespace BestPriceStore.DTOs
{
    public class AdminDashboardDTO
    {
        // Sales Summary
        public double TotalSalesYer { get; set; }
        public double TotalSalesSar { get; set; }

        // Orders Summary
        public int TotalOrders { get; set; }
        public int PendingOrdersCount { get; set; }
        public int ProcessingOrdersCount { get; set; }
        public int ShippedOrdersCount { get; set; }
        public int DeliveredOrdersCount { get; set; }
        public int CancelledOrdersCount { get; set; }

        // Inventory & Representatives Summary
        public int TotalActiveProducts { get; set; }
        public int OutOfStockProductsCount { get; set; }
        public int TotalRepresentatives { get; set; }
        public int ActiveRepresentatives { get; set; }
    }
}
