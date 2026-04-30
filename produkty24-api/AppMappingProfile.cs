using AutoMapper;
using Produkty24_API.Models.DTO.Clients;
using Produkty24_API.Models.DTO.ExchangeRates;
using Produkty24_API.Models.DTO.Manufacturers;
using Produkty24_API.Models.DTO.Orders;
using Produkty24_API.Models.DTO.OrdersItems;
using Produkty24_API.Models.DTO.Payments;
using Produkty24_API.Models.DTO.StockArrivals;
using Produkty24_API.Models.DTO.StockItems;
using Produkty24_API.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produkty24_API
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            CreateMap<ClientEntity, AllClientsDto>();
            CreateMap<ClientCreateDto, ClientEntity>();
            CreateMap<ClientEditDto, ClientEntity>().ReverseMap();
            CreateMap<ClientEntity, ClientProfileDto>();
            CreateMap<ExchangeRateEntity, AllExchangeRatesDto>();
            CreateMap<ExchangeRateCreateDto, ExchangeRateEntity>();
            CreateMap<ExchangeRateEditDto, ExchangeRateEntity>().ReverseMap();
            CreateMap<ManufacturerEntity, ManufacturerEditDto>().ReverseMap();
            CreateMap<ManufacturerCreateDto, ManufacturerEntity>();
            CreateMap<OrderEntity, OrderDto>();
            CreateMap<OrderEntity, OrderDetailsDto>();
            CreateMap<OrderCreateDto, OrderEntity>();
            CreateMap<OrderEditDto, OrderEntity>();
            CreateMap<StockItemEntity, AllStockItemsDto>();
            CreateMap<StockItemCreateDto, StockItemEntity>();
            CreateMap<StockItemEntity, StockItemEditDto>().ReverseMap();
            CreateMap<StockArrivalEntity, AllStockArrivalsDto>();
            CreateMap<StockArrivalCreateDto, StockArrivalEntity>();
            CreateMap<StockArrivalEntity, StockArrivalEditDto>().ReverseMap();
            CreateMap<OrderItemEntity, AllOrderItemsDto>();
            CreateMap<OrderItemCreateDto, OrderItemEntity>();
            CreateMap<OrderItemEntity, OrderItemEditDto>().ReverseMap();
            CreateMap<PaymentEntity, AllPaymentsDto>();
            CreateMap<PaymentCreateDto, PaymentEntity>();
            CreateMap<PaymentEditDto, PaymentEntity>().ReverseMap();
        }
    }
}
