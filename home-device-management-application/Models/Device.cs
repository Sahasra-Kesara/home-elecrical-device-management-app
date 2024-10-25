namespace home_device_management_application.Models
{
    public class Device
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; } // This should store the image path
        public string Brand { get; set; }
        public DateTime BuyDate { get; set; }
        public int WarrantyPeriod { get; set; } // in months
        public int ServicePeriod { get; set; } // in months
        public string CustomerServiceMobileNumber { get; set; }
        public DateTime LastServiceDate { get; set; }
        public DateTime RepairDueDate { get; set; }

        // Add methods to calculate when warranty, service, and repair notifications should be triggered
        public bool IsWarrantyNearEnd()
        {
            return DateTime.Now >= BuyDate.AddMonths(WarrantyPeriod).AddDays(-30); // Alert 30 days before the end
        }

        public bool IsServiceDue()
        {
            return DateTime.Now >= LastServiceDate.AddMonths(ServicePeriod);
        }

        public bool IsRepairDue()
        {
            return DateTime.Now >= RepairDueDate;
        }
    }
}
