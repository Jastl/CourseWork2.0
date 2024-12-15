using System.Numerics;
using FileStorage;
using BusinessLogic;

namespace Registry 
{
    static class Program 
    {
        private static Repository<Doctor> doctorRepository = new();
        private static Repository<Patient> patientRepository = new();
        private static Repository<Appointment> appointmentRepository = new();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            LoadData();
            while (true)
            {
                Console.WriteLine("Меню:");
                Console.WriteLine("1. Додати лікаря");
                Console.WriteLine("2. Додати пацієнта");
                Console.WriteLine("3. Створити прийом");
                Console.WriteLine("4. Показати всіх лікарів");
                Console.WriteLine("5. Показати всіх пацієнтів");
                Console.WriteLine("6. Видалити лікаря");
                Console.WriteLine("7. Видалити пацієнта");
                Console.WriteLine("8. Знайти лікаря за ім'ям");
                Console.WriteLine("9. Знайти пацієнта за ім'ям");
                Console.WriteLine("10. Оновити дані лікаря");
                Console.WriteLine("11. Оновити дані пацієнта");
                Console.WriteLine("12. Додати розклад для лікаря");
                Console.WriteLine("13. Змінити розклад лікаря");
                Console.WriteLine("14. Отримати розклад лікаря");
                Console.WriteLine("15. Показати записи лікаря");
                Console.WriteLine("16. Показати електронну книжку пацієнта");
                Console.WriteLine("17. Змінити електронну книжку пацієнта");
                Console.WriteLine("0. Вихід");
                Console.Write("Ваш вибір: ");

                switch (Console.ReadLine())
                {
                    case "1": AddDoctor(); break;
                    case "2": AddPatient(); break;
                    case "3": ShowAllDoctors(); Console.WriteLine(); ShowAllPatients(); CreateAppointment(); break;
                    case "4": ShowAllDoctors(); break;
                    case "5": ShowAllPatients(); break;
                    case "6": ShowAllDoctors(); DeleteDoctor(); break;
                    case "7": ShowAllPatients(); DeletePatient(); break;
                    case "8": FindDoctorByName(); break; //TODO
                    case "9": FindPatientByName(); break; //TODO
                    case "10": ShowAllDoctors(); UpdateDoctor(); break;
                    case "11": ShowAllPatients(); UpdatePatient(); break;
                    case "12": ShowAllDoctors(); SetDoctorSchedule(); break;
                    case "13": ShowAllDoctors(); UpdateDoctorSchedule(); break;
                    case "14": ShowAllDoctors(); GetDoctorSchedule(); break;
                    case "15": ShowAllDoctors(); ShowDoctorAppointments(); break;
                    case "16": ShowAllPatients(); ShowPatientRecord(); break;
                    case "17": ShowAllPatients(); UpdatePatientRecord(); break;
                    case "0": return;
                    default: Console.WriteLine("Невірний вибір."); break;
                }
            }
        }
        private static void UpdatePatientRecord()
        {
            Console.Write("Введіть індекс пацієнта: ");
            int index;
            int.TryParse(Console.ReadLine(), out index);
            Patient patient;
            try
            {
                patient = patientRepository.GetAll()[index];
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Пацієнта з таким індексом не знайдено");
                return;
            }

            Console.WriteLine($"Поточні записи для пацієнта {patient.Name}:");
            if (patient.Record.Any())
            {
                foreach (var record in patient.Record)
                {
                    Console.WriteLine($"- {record}");
                }
            }
            else
            {
                Console.WriteLine("Записів немає.");
            }

            Console.WriteLine("1. Додати новий запис");
            Console.WriteLine("2. Завершити");
            Console.Write("Ваш вибір: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Введіть новий запис: ");
                    var newRecord = Console.ReadLine();
                    patient.Record.Add(newRecord);
                    Console.WriteLine("Новий запис додано.");
                    SaveData();
                    break;

                case "2":
                    Console.WriteLine("Роботу завершено.");
                    break;

                default:
                    Console.WriteLine("Невірний вибір.");
                    break;
            }
        }
        private static void UpdatePatient()
        {
            Console.Write("Введіть індекс пацієнта: ");
            int index;
            int.TryParse(Console.ReadLine(), out index);
            Patient patient;
            try
            {
                patient = patientRepository.GetAll()[index];
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Пацієнта з таким індексом не знайдено");
                return;
            }

            Console.Write("Введіть нове ім'я пацієнта: ");
            patient.Name = Console.ReadLine();

            Console.Write("Введіть нову дату народження (рррр-мм-дд): ");
            if (DateTime.TryParse(Console.ReadLine(), out var dob))
            {
                patient.DateOfBirth = dob;
                Console.WriteLine("Дані пацієнта оновлено.");
                SaveData();
            }
            else
            {
                Console.WriteLine("Невірний формат дати.");
            }
        }

        private static void UpdateDoctorSchedule()
        {
            Console.Write("Введіть індекс лікаря для оновлення розкладу: ");
            int index;
            int.TryParse(Console.ReadLine(), out index);
            Doctor doctor;
            try
            {
                doctor = doctorRepository.GetAll()[index];
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Лікаря з таким індексом не знайденою");
                return;
            }

            Console.Write("Введіть новий час початку робочого дня (гг:хх): ");
            if (TimeOnly.TryParse(Console.ReadLine(), out var startTime))
            {
                Console.Write("Введіть новий час закінчення робочого дня (гг:хх): ");
                if (TimeOnly.TryParse(Console.ReadLine(), out var endTime))
                {
                    doctor.WorkSchedule = new DoctorSchedule { StartTime = startTime, EndTime = endTime };
                    Console.WriteLine("Розклад лікаря оновлено.");
                    SaveData();
                }
                else
                {
                    Console.WriteLine("Невірний формат часу.");
                }
            }
            else
            {
                Console.WriteLine("Невірний формат часу.");
            }
        }

        private static void ShowDoctorAppointments()
        {
            Console.Write("Введіть індекс лікаря: ");
            int index;
            int.TryParse(Console.ReadLine(), out index);
            Doctor doctor;
            try
            {
                doctor = doctorRepository.GetAll()[index];
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Лікаря з таким індексом не знайдено");
                return;
            }

            var appointments = appointmentRepository.GetAll().Where(a => a.DoctorId == doctor.Id).ToList();
            if (appointments.Any())
            {
                foreach (var appt in appointments)
                {
                    var patient = patientRepository.GetAll().FirstOrDefault(p => p.Id == appt.PatientId);
                    Console.WriteLine($"Прийом: {appt.Date}, Пацієнт: {patient?.Name ?? "Невідомий"}");
                }
            }
            else
            {
                Console.WriteLine("Записів немає.");
            }
        }

        private static void ShowPatientRecord()
        {
            Console.Write("Введіть індекс пацієнта: ");
            int index;
            int.TryParse(Console.ReadLine(), out index);
            Patient patient;
            try
            {
                patient = patientRepository.GetAll()[index];
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Пацієнта з таким індексом не знайдено");
                return;
            }

            Console.WriteLine($"Електронна книжка пацієнта {patient.Name}:");

            if (patient.Record != null && patient.Record.Any())
            {
                foreach (var record in patient.Record)
                {
                    Console.WriteLine($"- {record}");
                }
            }
            else
            {
                Console.WriteLine("Записів немає.");
            }
        }

        private static void UpdateDoctor()
        {
            Console.Write("Введіть індекс лікаря: ");
            int index;
            int.TryParse(Console.ReadLine(), out index);
            Doctor doctor;
            try
            {
                doctor = doctorRepository.GetAll()[index];
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Лікаря з таким індексом не знайдено");
                return;
            }

            Console.Write("Введіть нове ім'я лікаря: ");
            doctor.Name = Console.ReadLine();

            Console.WriteLine("Оберіть нову спеціалізацію:");
            foreach (var spec in Enum.GetValues(typeof(Specialization)))
            {
                Console.WriteLine($"{(int)spec}: {spec}");
            }
            if (Enum.TryParse(Console.ReadLine(), out Specialization newSpec))
            {
                doctor.Specialization = newSpec;
                Console.WriteLine("Дані лікаря оновлено.");
                SaveData();
            }
            else
            {
                Console.WriteLine("Невірний вибір спеціалізації.");
            }
        }

        private static void SetDoctorSchedule()
        {
            Console.Write("Введіть індекс лікаря для встановлення розкладу: ");
            int index;
            int.TryParse(Console.ReadLine(), out index);
            Doctor doctor;
            try
            {
                doctor = doctorRepository.GetAll()[index];
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Лікаря з таким індексом не існує.");
                return;
            }

            Console.Write("Введіть час початку робочого дня (гг:хх): ");
            if (!TimeOnly.TryParse(Console.ReadLine(), out var startTime))
            {
                Console.WriteLine("Невірний формат часу.");
                return;
            }

            Console.Write("Введіть час закінчення робочого дня (гг:хх): ");
            if (!TimeOnly.TryParse(Console.ReadLine(), out var endTime))
            {
                Console.WriteLine("Невірний формат часу.");
                return;
            }

            doctor.WorkSchedule = new DoctorSchedule { StartTime = startTime, EndTime = endTime };
            Console.WriteLine("Розклад лікаря оновлено.");
            SaveData();
        }

        private static void GetDoctorSchedule()
        {
            Console.Write("Введіть індекс лікаря: ");
            int index;
            int.TryParse(Console.ReadLine(), out index);
            Doctor doctor;
            try
            {
                doctor = doctorRepository.GetAll()[index];
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Лікаря з таким індексом не знайденою");
                return;
            }

            if (doctor?.WorkSchedule != null)
            {
                Console.WriteLine($"Розклад лікаря {doctor.Name}: {doctor.WorkSchedule.StartTime} - {doctor.WorkSchedule.EndTime}");
            }
            else
            {
                Console.WriteLine("Розклад для цього лікаря не знайдено.");
            }
        }

        private static void DeleteDoctor()
        {
            Console.Write("Введіть індекс лікаря для видалення: ");
            int index;
            int.TryParse(Console.ReadLine(), out index);
            try
            {
                doctorRepository.Remove(index);
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Лікаря з таким індексом не існує.");
            }
        }

        private static void DeletePatient()
        {
            Console.Write("Введіть індекс пацієнта для видалення: ");
            int index;
            int.TryParse(Console.ReadLine(), out index);
            try
            {
                patientRepository.Remove(index);
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Пацієнта з таким індексом не існує.");
            }
        }
        private static void FindDoctorByName()
        {
            Console.Write("Введіть ім'я лікаря: ");
            var name = Console.ReadLine();
            foreach (var doctor in doctorRepository.GetAll())
            {
                if (doctor.Name.Contains(name))
                {
                    Console.WriteLine($"Index: {doctorRepository.IndexOf(doctor)}, ID: {doctor.Id}, Ім'я: {doctor.Name}, Спеціалізація: {doctor.Specialization}");
                }
            }
        }

        private static void FindPatientByName()
        {
            Console.Write("Введіть ім'я пацієнта: ");
            var name = Console.ReadLine();
            foreach (var patient in patientRepository.GetAll())
            {
                if (patient.Name.Contains(name))
                {
                    Console.WriteLine($"Index: {patientRepository.IndexOf(patient)}, ID: {patient.Id}, Ім'я: {patient.Name}, Спеціалізація: {patient.DateOfBirth}");
                }
            }
        }
        private static void AddDoctor()
        {
            Console.Write("Введіть ім'я лікаря: ");
            var name = Console.ReadLine();
            try
            {
                Validation.ValidateName(name);
            }
            catch (ArithmeticException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.WriteLine("Оберіть спеціалізацію:");
            foreach (var spec in Enum.GetValues(typeof(Specialization)))
            {
                Console.WriteLine($"{(int)spec}: {spec}");
            }

            if (!Enum.TryParse(Console.ReadLine(), out Specialization specialization))
            {
                Console.WriteLine("Невірний вибір спеціалізації.");
                return;
            }

            doctorRepository.Add(new Doctor { Name = name, Specialization = specialization });
            Console.WriteLine("Лікаря додано.");
            SaveData();
        }


        private static void AddPatient()
        {
            Console.Write("Введіть ім'я пацієнта: ");
            string name = Console.ReadLine();
            try
            {
                Validation.ValidateName(name);
            }
            catch (ArithmeticException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.Write("Введіть дату народження (рррр-мм-дд): ");
            if (!DateTime.TryParse(Console.ReadLine(), out var dateOfBirth))
            {
                Console.WriteLine("Невірний формат дати.");
                return;
            }

            try
            {
                Validation.ValidateDateOfBirth(dateOfBirth);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            patientRepository.Add(new Patient { Name = name, DateOfBirth = dateOfBirth });
            Console.WriteLine("Пацієнта додано.");
            SaveData();
        }


        private static void CreateAppointment()
        {
            Console.Write("Індекс лікаря: ");
            int doctorIndex;
            int.TryParse(Console.ReadLine(), out doctorIndex);
            Console.Write("Індекс пацієнта: ");
            int patientIndex;
            int.TryParse(Console.ReadLine(), out patientIndex);
            Console.Write("Дата та час прийому (рррр-мм-дд гг:хх): ");

            try
            { 
                if (DateTime.TryParse(Console.ReadLine(), out var dateTime))
                {
                    try
                    {
                        Validation.ValidateAppointmentDate(dateTime);
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                        return;
                    }
                    var doctor = doctorRepository.GetAll()[doctorIndex];
                    if (doctor == null || doctor.WorkSchedule == null)
                    {
                        Console.WriteLine("Лікаря не знайдено або розклад не встановлено.");
                        return;
                    }
                    if (dateTime.TimeOfDay < doctor.WorkSchedule.StartTime.ToTimeSpan() || dateTime.TimeOfDay >= doctor.WorkSchedule.EndTime.ToTimeSpan())
                    {
                        Console.WriteLine("Прийом поза межами робочого часу.");
                        return;
                    }

                    appointmentRepository.Add(new Appointment
                    {
                        DoctorId = doctor.Id,
                        PatientId = patientRepository.GetAll()[patientIndex].Id,
                        Date = dateTime
                    });

                    Console.WriteLine("Прийом створено.");
                    SaveData();
                }
                else
                {
                    Console.WriteLine("Невірний формат дати.");
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Якийсь з індексів не вірний");
            }
        }


        private static void ShowAllDoctors()
        {
            foreach (var doctor in doctorRepository.GetAll())
                Console.WriteLine($"Index: {doctorRepository.IndexOf(doctor)}, ID лікаря: {doctor.Id}, Ім'я: {doctor.Name}, Спеціалізація: {doctor.Specialization}");
        }

        private static void ShowAllPatients()
        {
            foreach (var patient in patientRepository.GetAll())
                Console.WriteLine($"Index: {patientRepository.IndexOf(patient)}, ID пацієнта: {patient.Id}, Ім'я: {patient.Name}, Дата народження: {patient.DateOfBirth:yyyy-MM-dd}");
            }

        private static void SaveData()
        {
            FileStorage<Doctor>.Save("doctors.json", doctorRepository.GetAll());
            FileStorage<Patient>.Save("patients.json", patientRepository.GetAll());
            FileStorage<Appointment>.Save("appointments.json", appointmentRepository.GetAll());
        }

        private static void LoadData()
        {
            doctorRepository = new Repository<Doctor>();
            patientRepository = new Repository<Patient>();
            appointmentRepository = new Repository<Appointment>();

            if (FileStorage<Doctor>.Load("doctors.json") != null) 
                foreach (var doctor in FileStorage<Doctor>.Load("doctors.json"))
                    doctorRepository.Add(doctor);

            if (FileStorage<Patient>.Load("patients.json") != null) 
                foreach (var patient in FileStorage<Patient>.Load("patients.json"))
                    patientRepository.Add(patient);

            if (FileStorage<Appointment>.Load("appointments.json") != null) 
                foreach (var appointment in FileStorage<Appointment>.Load("appointments.json"))
                    appointmentRepository.Add(appointment);
        }
    }
}