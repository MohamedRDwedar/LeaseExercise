using AutoMapper;
using LeaseExercise.Domain.Enums;
using LeaseExercise.Domain.Models;
using LeaseExercise.Services.ViewModels;
using System;
using System.Linq;

namespace LeaseExercise.Services.Mapping
{
    class LeaseExerciseProfile : Profile
    {
        /// <summary>
        /// the default auto mapper profile to map between the data models and view models 
        /// </summary>
        public LeaseExerciseProfile()
        {

        }
    }
}
