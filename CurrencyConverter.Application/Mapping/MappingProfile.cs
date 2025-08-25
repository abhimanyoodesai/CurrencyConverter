using AutoMapper;
using CurrencyConverter.Application.DTOs;
using CurrencyConverter.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CurrencyConverter.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CurrencyRate, CurrencyRateResponse>();
            CreateMap<CurrencyConversion, ConversionResponse>();
        }
    }
}
