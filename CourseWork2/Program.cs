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
            Console.InputEncoding = System.Text.Encoding.UTF8;
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
                Console.WriteLine("10. Змінити дані лікаря");
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
                    case "3": ShowAllDoctors(); ShowAllPatients(); CreateAppointment(); break;
                    case "4": ShowAllDoctors(); break;
                    case "5": ShowAllPatients(); break;
                    case "6": DeleteDoctor(); break;
                    case "7": DeletePatient(); break;
                    case "8": FindDoctorByName(); break;
                    case "9": FindPatientByName(); break;
                    case "10": UpdateDoctor(); break;
                    case "11": UpdatePatient(); break;
                    case "12": SetDoctorSchedule(); break;
                    case "13": UpdateDoctorSchedule(); break;
                    case "14": GetDoctorSchedule(); break;
                    case "15": ShowDoctorAppointments(); break;
                    case "16": ShowPatientRecord(); break;
                    case "17": UpdatePatientRecord(); break;
                    case "0": return;
                    default: Console.WriteLine("Невірний вибір."); break;
                }
            }
        }
        private static void UpdatePatientRecord()
        {
            Console.Write("Введіть ім'я пацієнта: ");
            var name = Console.ReadLine();
            var patient = patientRepository.GetAll().FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (patient == null)
            {
                Console.WriteLine("Пацієнта не знайдено.");
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
            Console.Write("Введіть ім'я пацієнта для оновлення: ");
            var name = Console.ReadLine();
            var patient = patientRepository.GetAll().FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (patient == null)
            {
                Console.WriteLine("Пацієнта не знайдено.");
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
            Console.Write("Введіть ім'я лікаря для оновлення розкладу: ");
            var name = Console.ReadLine();
            var doctor = doctorRepository.GetAll().FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (doctor == null)
            {
                Console.WriteLine("Лікаря не знайдено.");
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
            Console.Write("Введіть ім'я лікаря: ");
            var name = Console.ReadLine();
            var doctor = doctorRepository.GetAll().FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (doctor == null)
            {
                Console.WriteLine("Лікаря не знайдено.");
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
            Console.Write("Введіть ім'я пацієнта: ");
            var name = Console.ReadLine();
            var patient = patientRepository.GetAll().FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (patient == null)
            {
                Console.WriteLine("Пацієнта не знайдено.");
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
            Console.Write("Введіть ім'я лікаря для оновлення: ");
            var name = Console.ReadLine();
            var doctor = doctorRepository.GetAll().FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (doctor == null)
            {
                Console.WriteLine("Лікаря не знайдено.");
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

        // Manage patient's medical record
        private static void ManagePatientRecords()
        {
            Console.Write("Введіть ім'я пацієнта: ");
            var name = Console.ReadLine();
            var patient = patientRepository.GetAll().FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (patient == null)
            {
                Console.WriteLine("Пацієнта не знайдено.");
                return;
            }

            Console.Write("Введіть нові дані пацієнта: ");
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

        // Add or update doctor schedule
        private static void SetDoctorSchedule()
        {
            Console.Write("Введіть ім'я лікаря для встановлення розкладу: ");
            var name = Console.ReadLine();
            var doctor = doctorRepository.GetAll().FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (doctor == null)
            {
                Console.WriteLine("Лікаря не знайдено.");
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

        // Display doctor's schedule
        private static void GetDoctorSchedule()
        {
            Console.Write("Введіть ім'я лікаря: ");
            var name = Console.ReadLine();
            var doctor = doctorRepository.GetAll().FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

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
            Console.Write("Введіть ім'я лікаря для видалення: ");
            var name = Console.ReadLine();
            var doctor = doctorRepository.GetAll().FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (doctor != null)
            {
                doctorRepository.Remove(doctor.Id);
                Console.WriteLine("Лікаря видалено.");
                SaveData();
            }
            else
                Console.WriteLine("Лікаря не знайдено.");
        }

        private static void DeletePatient()
        {
            Console.Write("Введіть ім'я пацієнта для видалення: ");
            var name = Console.ReadLine();
            var patient = patientRepository.GetAll().FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (patient != null)
            {
                patientRepository.Remove(patient.Id);
                Console.WriteLine("Пацієнта видалено.");
                SaveData();
            }
            else
                Console.WriteLine("Пацієнта не знайдено.");
        }
        private static void FindDoctorByName()
        {
            Console.Write("Введіть ім'я лікаря: ");
            var name = Console.ReadLine();
            var doctors = doctorRepository.GetAll().Where(d => d.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            if (doctors.Any())
                foreach (var doctor in doctors)
                    Console.WriteLine($"ID: {doctor.Id}, Ім'я: {doctor.Name}, Спеціалізація: {doctor.Specialization}");
            else
                Console.WriteLine("Лікаря не знайдено.");
        }

        private static void FindPatientByName()
        {
            Console.Write("Введіть ім'я пацієнта: ");
            var name = Console.ReadLine();
            var patients = patientRepository.GetAll().Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            if (patients.Any())
                foreach (var patient in patients)
                    Console.WriteLine($"ID: {patient.Id}, Ім'я: {patient.Name}, Дата народження: {patient.DateOfBirth:yyyy-MM-dd}");
            else
                Console.WriteLine("Пацієнта не знайдено.");
        }
        private static void AddDoctor()
        {
            Console.Write("Введіть ім'я лікаря: ");
            var name = Console.ReadLine();
            Validation.ValidateName(name);

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
            var name = Console.ReadLine();
            Validation.ValidateName(name);

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
            Console.Write("ID лікаря: ");
            var doctorId = Console.ReadLine();
            Console.Write("ID пацієнта: ");
            var patientId = Console.ReadLine();
            Console.Write("Дата та час прийому (рррр-мм-дд гг:хх): ");

            if (DateTime.TryParse(Console.ReadLine(), out var dateTime))
            {
                var doctor = doctorRepository.GetById(Guid.Parse(doctorId));
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
                    PatientId = Guid.Parse(patientId),
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


        private static void ShowAllDoctors()
        {
            foreach (var doctor in doctorRepository.GetAll())
                Console.WriteLine($"ID лікаря: {doctor.Id}, Ім'я: {doctor.Name}, Спеціалізація: {doctor.Specialization}");
        }

        private static void ShowAllPatients()
        {
            foreach (var patient in patientRepository.GetAll())
                Console.WriteLine($"ID пацієнта: {patient.Id}, Ім'я: {patient.Name}, Дата народження: {patient.DateOfBirth:yyyy-MM-dd}");
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