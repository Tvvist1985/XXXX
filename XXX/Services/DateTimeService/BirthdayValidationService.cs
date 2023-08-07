using System.ComponentModel.DataAnnotations;

namespace Services.DateTimeService
{
    public enum NumberErrorMassege : byte
    {        
        ErrorExceedingRangeDays,
        ErrorExceedingRangeYears,
        ErrorIfYearFieldNotNull,
        ErrorIfMonthFieldNotNull,
        ErrorIfAllFieldNotNull,
        ErrorNegativeDay,

    }
    public class BirthdayValidationService 
    {      
        public string ErrorMessage { get; set; }
        [Required]
        public short? Day { get; set; } = null;
        [Required]
        [Range(1, 12)]
        public short? Month { get; set; } = null;
        [Required]
        public short? Year { get; set; } = null;
        public DateTime DateOfBirth { get; set; }        
        public int Age { get; set; }

        //Выбор и инициализация сообщения об ошибке
        //
        public void ValidationModel(NumberErrorMassege? numberError)
        {            
            switch(numberError)
            {
                case NumberErrorMassege.ErrorExceedingRangeYears:
                    ErrorMessage = $"The field Year must be between 1933 and {DateTime.Now.Year - 18}.";
                    break;
                case NumberErrorMassege.ErrorExceedingRangeDays:
                    ErrorMessage = $"The field Days must be between 1 and {DateTime.DaysInMonth(2000, 1)}.";                 
                    break;
                case NumberErrorMassege.ErrorIfMonthFieldNotNull:
                    ErrorMessage = $"The field Days must be between 1 and {DateTime.DaysInMonth(2000, (short)Month)}.";
                    break;
                case NumberErrorMassege.ErrorIfYearFieldNotNull:
                    ErrorMessage = $"The field Days must be between 1 and {DateTime.DaysInMonth((short)Year, 1)}.";
                    break;
                case NumberErrorMassege.ErrorIfAllFieldNotNull:
                    ErrorMessage = $"The field Days must be between 1 and {DateTime.DaysInMonth((short)Year, (short)Month)}.";
                    break;
                    //Сообщение об отрицательном значении такое иначе придёться опять переписывать в диапазоны по дням
                case NumberErrorMassege.ErrorNegativeDay:
                    ErrorMessage = $"The day field cannot be less than 1.";
                    break;
            }
        }

        //Метод расчёта возраста и инициализация по дате дня рождения
        public int Foo()
        {
            DateOfBirth = new((short)Year, (short)Month, (short)Day);
            DateTime nowDate = DateTime.Today;

            //Перевод даты в возраст
            Age = nowDate.Year - DateOfBirth.Year;
            if (DateOfBirth > nowDate.AddYears(-Age)) Age--;

            return Age;
        }

        //ПРолучаю позраст в годах
        public int GetAge(DateTime BirthDay)
        {
            DateTime nowDate = DateTime.Today;

            //Перевод даты в возраст
            Age = nowDate.Year - BirthDay.Year ;
            if (BirthDay > nowDate.AddYears(-Age)) Age--;

            return Age;
        }

    }
}
