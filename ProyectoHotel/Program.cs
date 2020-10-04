using System;
using System.Configuration;
using System.Data.SqlClient;

namespace ProyectoHotel
{
    class Program
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["HOTEL"].ConnectionString;
        static SqlConnection conexion = new SqlConnection(connectionString);
        static string cadena;
        static SqlCommand comando;
        static SqlDataReader registros;

        static void menu()
        {
            string dni;
            bool salir=false;
            string opcion;
            do
            {
                Console.WriteLine("\nEste es el menu de nuestro hotel\nEstas son las opciones que le mostramos. " +
                                  "Introduce el numero de al lado de la accion que quieras ejecutar\n\n\n1.-Registrarte cliente\n" +
                                  "2.-Editar los datos del cliente\n3.-Hacer el Check In\n" +
                                  "4.-Hacer el Check Out\n5.-Salir\n6.-Ver Habitaciones");
                opcion = Console.ReadLine();
                switch (opcion)
                {
                    case "1":
                        Console.Clear();
                        RegistrarCliente();
                        break;
                    case "2":
                        Console.Clear();
                        dni = ComprobarDni();
                        if (dni != "5")
                        {
                           EditarCliente(dni);
                        }
                        break;
                    case "3":
                        Console.Clear();
                        CheckIn();
                        break;
                    case "4":
                        Console.Clear();
                        dni = ComprobarDni();
                        if (dni != "5")
                        {
                           CheckOut(dni);
                        }
                        break;
                    case "5":
                        salir = true;
                        break;
                    case "6":
                        Console.Clear();
                        VerHabitaciones();
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Has introducido una opcion incorrecta vuelve a intentarlo\n");
                        break;
                }
            } while (salir==false);
        }
        static void RegistrarCliente()
        {
            char caracter;
            bool dniNoValido = false,ok=false,dniNoRepetido=true;
            string dni;
            do {
                Console.WriteLine("Introduce tu DNI o pulsa 5 para salir");
                dni = Console.ReadLine();
                if (dni.Length == 9)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        caracter = Convert.ToChar(dni.Substring(i, 1));
                        if (char.IsLetter(caracter))
                        {
                            ok = false;
                            dniNoValido = true;

                        }
                        else
                        {
                            ok = true;
                        }
                    }
                    if (ok)
                    {
                        string letra;

                        letra = (dni.Substring(8, 1));
                        caracter = Convert.ToChar(letra.ToLower());
                        
                        if (char.IsLetter(caracter))
                        {
                            conexion.Open();
                            cadena = $"SELECT DNI from Clientes where DNI='{dni}'";
                            comando = new SqlCommand(cadena, conexion);
                            registros = comando.ExecuteReader();
                            while (registros.Read())
                            {
                                dniNoRepetido = false;
                            }
                            conexion.Close();
                            if (dniNoRepetido)
                            {
                                Console.WriteLine("Introduce tu nombre");
                                string nombre = Console.ReadLine();
                                Console.WriteLine("Introduce tu apellido");
                                string apellido = Console.ReadLine();
                                conexion.Open();
                                cadena = $"INSERT INTO Clientes(Nombre,Apellidos,DNI) VALUES ('{nombre}','{apellido}','{dni}')";
                                comando = new SqlCommand(cadena, conexion);
                                comando.ExecuteNonQuery();
                                conexion.Close();
                                dniNoValido = false;
                            }
                            else
                            {
                                dniNoValido = true;
                                Console.WriteLine("Ya eres cliente no hace falta que te registres");
                            }
                        }
                        else
                        {
                            Console.WriteLine("El numero de DNI que has introducido no es valido. Vuelve a introducir el DNI");
                            dniNoValido = true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("El numero de DNI que has introducido no es valido. Vuelve a introducir el DNI");
                        dniNoValido = true;
                    }
                }
                else if (dni == "5") dniNoValido = false;
                else
                {
                    Console.WriteLine("El numero de DNI que has introducido no es valido. Vuelve a introducir el DNI");
                    dniNoValido = true;
                }
            } while (dniNoValido == true);
            
            
        }

        static public string ComprobarDni()
        {
            string dni;
            bool dniCorrecto = false;
            do
            {
                Console.WriteLine("Introduce tu DNI o pulsa 5 si quieres salir");
                dni = Console.ReadLine();
                if (dni == "5")
                {
                    dniCorrecto = true;
                }
                else
                {

                    conexion.Open();
                    cadena = $"SELECT DNI FROM Clientes where DNI='{dni}'";
                    comando = new SqlCommand(cadena, conexion);
                    registros = comando.ExecuteReader();
                    while (registros.Read())
                    {
                        dniCorrecto = true;
                    }
                    conexion.Close();
                }
                
            } while (!dniCorrecto);

            return dni;
        }
        static public void EditarCliente(string dni)
        {
            if (dni != "5")
            { 
                string nombre, apellido;
                Console.WriteLine("Introduce un nuevo nombre");
                nombre = Console.ReadLine();
                Console.WriteLine("Introduce un nuevo apellido");
                apellido = Console.ReadLine();
                conexion.Open();
                cadena = $"UPDATE Clientes SET Nombre='{nombre}',Apellidos='{apellido}' where DNI='{dni}'";
                comando = new SqlCommand(cadena, conexion);
                comando.ExecuteNonQuery();
                conexion.Close();
            }
        }
        
        static public void CheckIn()
        {
            string dni;
            string habitacionElegida;
            bool habitacionDisponible=false;
            dni=ComprobarDni();
            if (dni!="5")
            {
                conexion.Open();
                cadena = "SELECT CodHabitacion from Habitaciones where Estado='Libre'";
                comando = new SqlCommand(cadena, conexion);
                registros = comando.ExecuteReader();
                while (registros.Read())
                {
                    Console.WriteLine(registros["CodHabitacion"].ToString());
                }
                conexion.Close();

                Console.WriteLine("Introduce la habitacion que quieras elegir");
                habitacionElegida=Console.ReadLine();

                conexion.Open();
                cadena = $"SELECT CodHabitacion from Habitaciones where Estado ='Libre'";
                comando = new SqlCommand(cadena, conexion);
                registros = comando.ExecuteReader();
                while (registros.Read())
                {
                    if (registros["CodHabitacion"].ToString() == habitacionElegida)
                    {
                        habitacionDisponible = true;
                    }
                   
                }

                conexion.Close();
                if (habitacionDisponible)
                {
                    conexion.Open();
                    cadena = $"UPDATE Habitaciones SET Estado='Ocupado'where CodHabitacion='{Convert.ToInt32( habitacionElegida)}'";
                    comando = new SqlCommand(cadena, conexion);
                    comando.ExecuteNonQuery();
                    conexion.Close();

                    conexion.Open();
                    cadena = $"INSERT INTO Reservas(FechaCheckIn,CodHabitacion,DNICliente) VALUES ('{DateTime.Now}','{habitacionElegida}','{dni}')";
                    comando = new SqlCommand(cadena, conexion);
                    comando.ExecuteNonQuery();
                    conexion.Close();
                    Console.WriteLine("Reserva efectuada");
                }

            }

        }

        static public void CheckOut(string dni)
        {
            bool Correcto=false,habitacionCorrecta=false;
            string habitacion="";
            conexion.Open();
            cadena = $"SELECT DNICliente FROM Reservas where DNICliente='{dni}' and FechaCheckOut is null";
            comando = new SqlCommand(cadena, conexion);
            registros = comando.ExecuteReader();
            while (registros.Read())
            {
                Correcto = true;
            }
            conexion.Close();
            if (Correcto)
            {
                Console.WriteLine("Estas son las habitaciones que tienes reservadas");
                conexion.Open();
                cadena = $"SELECT CodHabitacion from Reservas where DNICliente='{dni}'and FechaCheckOut is null";
                comando = new SqlCommand(cadena, conexion);
                registros = comando.ExecuteReader();
                while (registros.Read())
                {
                    Console.WriteLine(registros["CodHabitacion"].ToString());
                }
                conexion.Close();
                Console.WriteLine("De que habitacion quieres hacer el checkout");
                habitacion = Console.ReadLine();
                conexion.Open();
                cadena = $"SELECT CodHabitacion from Reservas where DNICliente='{dni}'";
                comando = new SqlCommand(cadena, conexion);
                registros = comando.ExecuteReader();
                while (registros.Read())
                {
                    
                    if (habitacion == registros["CodHabitacion"].ToString())
                    {
                        habitacionCorrecta = true;
                    }
                }
                conexion.Close();
                if (habitacionCorrecta)
                {
                    conexion.Open();
                    cadena = $"UPDATE Reservas SET FechaCheckOut='{DateTime.Now}'where DNICliente='{dni}' and CodHabitacion='{habitacion}'";
                    comando = new SqlCommand(cadena, conexion);
                    comando.ExecuteNonQuery();
                    conexion.Close();

                    conexion.Open();
                    cadena = $"UPDATE Habitaciones SET Estado='Libre'where CodHabitacion='{Convert.ToInt32(habitacion)}'";
                    comando = new SqlCommand(cadena, conexion);
                    comando.ExecuteNonQuery();
                    conexion.Close();
                }
                else Console.WriteLine("Esta habitacion no está a tu nombre");
            }
            else
            {
                Console.WriteLine("El DNI introducido no tiene ninguna habitacion reservada");
            }
        }
        static void VerHabitaciones()
        {
            string opcion;
            Console.WriteLine("Introduce alguna de las siguientes opciones:\n\t\t1.Ver listado de todas las habitaciones con nombre de su huésped o vacía\n" +
                "\t\t2.Ver listado de habitaciones ocupadas con el nombre de su huésped\n\t\t3.Ver listado de habitaciones vacías");
            opcion = Console.ReadLine();
            Console.WriteLine("\n");
            switch (opcion)
            {
                case "1":
                 conexion.Open();
            cadena = "SELECT Habitaciones.CodHabitacion,Habitaciones.Estado,Clientes.Nombre FROM Habitaciones FULL OUTER JOIN Reservas ON Reservas.CodHabitacion = Habitaciones.CodHabitacion and Reservas.FechaCheckOut is null FULL OUTER JOIN Clientes ON Clientes.DNI = Reservas.DNICliente where Reservas.FechaCheckOut is null";
            comando = new SqlCommand(cadena, conexion);
            registros = comando.ExecuteReader();
            while (registros.Read())
            {
                Console.WriteLine(registros["CodHabitacion"].ToString() + "\t" + registros["Estado"].ToString() + "\t" + registros["Nombre"].ToString());
            }
                    Console.WriteLine("\n");
            conexion.Close();
                    break;
                case "2":
            conexion.Open();
            cadena = "SELECT  Habitaciones.CodHabitacion,Habitaciones.Estado,Clientes.Nombre from Habitaciones full outer join Reservas On Habitaciones.CodHabitacion = Reservas.CodHabitacion and Reservas.FechaCheckOut is null full outer join Clientes On Clientes.DNI = Reservas.DNICliente where Reservas.FechaCheckOut is null and Habitaciones.Estado = 'Ocupado'";
            comando = new SqlCommand(cadena, conexion);
            registros = comando.ExecuteReader();
            while (registros.Read())
            {
                Console.WriteLine(registros["CodHabitacion"].ToString() + "\t" + registros["Estado"].ToString() + "\t" + registros["Nombre"].ToString());
            }
            conexion.Close();
                    Console.WriteLine("\n");
                    break;
                case "3":
            conexion.Open();
            cadena = "SELECT  Habitaciones.CodHabitacion,Habitaciones.Estado from Habitaciones full outer join Reservas On Habitaciones.CodHabitacion = Reservas.CodHabitacion and Reservas.FechaCheckOut is null full outer join Clientes On Clientes.DNI = Reservas.DNICliente where Reservas.FechaCheckOut is null and Habitaciones.Estado = 'Libre'";
            comando = new SqlCommand(cadena, conexion);
            registros = comando.ExecuteReader();
            while (registros.Read())
            {
                Console.WriteLine(registros["CodHabitacion"].ToString() + "\t" + registros["Estado"].ToString());
            }
            conexion.Close();
                    Console.WriteLine("\n");
                    break;
                default:
                    Console.WriteLine("Has introducido un valor incorrecto");
                    break;
            }
        }
        static void Main(string[] args)
        {
            menu();
        }
    }
}
