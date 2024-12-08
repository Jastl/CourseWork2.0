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
            LoadData();
            while (true)
            {
                Console.WriteLine("Меню:");
                Console.WriteLine("1. Додати лікаря");
                Console.WriteLine("2. Додати пацієнта");
                Console.WriteLine("3. Створити прийом");
                Console.WriteLine("4. Показати всіх лікарів");
                Console.WriteLine("5. Показати всіх пацієнтів");
                Console.WriteLine("0. Вихід");
                Console.Write("Ваш вибір: ");

                switch (Console.ReadLine())
                {
                    case "1": AddDoctor(); break;
                    case "2": AddPatient(); break;
                    case "3": CreateAppointment(); break;
                    case "4": ShowAllDoctors(); break;
                    case "5": ShowAllPatients(); break;
                    case "0": return;
                    default: Console.WriteLine("Невірний вибір."); break;
                }
            }
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
            if (!Guid.TryParse(Console.ReadLine(), out var doctorId))
            {
                Console.WriteLine("Невірний формат ID.");
                return;
            }

            Console.Write("ID пацієнта: ");
            if (!Guid.TryParse(Console.ReadLine(), out var patientId))
            {
                Console.WriteLine("Невірний формат ID.");
                return;
            }

            Console.Write("Дата та час прийому (рррр-мм-дд гг:хх): ");
            if (!DateTime.TryParse(Console.ReadLine(), out var appointmentDate))
            {
                Console.WriteLine("Невірний формат дати.");
                return;
            }

            try
            {
                Validation.ValidateAppointmentDate(appointmentDate);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            if (appointmentRepository.GetAll().Count(a => a.DoctorId == doctorId && a.Date.Date == appointmentDate.Date) >= 5)
            {
                Console.WriteLine("Перевищено ліміт прийомів для лікаря на день.");
                return;
            }

            appointmentRepository.Add(new Appointment { DoctorId = doctorId, PatientId = patientId, Date = appointmentDate });
            Console.WriteLine("Прийом створено.");
            SaveData();
        }


        private static void ShowAllDoctors()
        {
            foreach (var doctor in doctorRepository.GetAll())
                Console.WriteLine($"ID: {doctor.Id}, Ім'я: {doctor.Name}, Спеціалізація: {doctor.Specialization}");
        }

        private static void ShowAllPatients()
        {
            foreach (var patient in patientRepository.GetAll())
                Console.WriteLine($"ID: {patient.Id}, Ім'я: {patient.Name}, Дата народження: {patient.DateOfBirth:yyyy-MM-dd}");
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