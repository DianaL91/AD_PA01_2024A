using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Protocolo
{
    public class Pedido
    {
        public string Comando { get; set; }
        public string[] Parametros { get; set; }
        /// Clase Pedido: representa la solicitud enviada por el cliente.
        public static Pedido Procesar(string mensaje)
        {
            var partes = mensaje.Split(' ');
            return new Pedido
            {
                Comando = partes[0].ToUpper(),
                Parametros = partes.Skip(1).ToArray()
            };
        }

        public override string ToString()
        {
            return $"{Comando} {string.Join(" ", Parametros)}";
        }
    }
    // Clase Respuesta: representa la respuesta enviada por el servidor.
    public class Respuesta
    {
        public string Estado { get; set; }
        public string Mensaje { get; set; }

        public override string ToString()
        {
            return $"{Estado} {Mensaje}";
        }
    }

    public class Protocolo
    {
        private TcpClient cliente;
        private NetworkStream flujo;

        public Protocolo(string ip, int puerto)
        {
            cliente = new TcpClient(ip, puerto);
            flujo = cliente.GetStream();
        }


        public Respuesta ResolverPedido(Pedido pedido)
        {
            return HazOperacion(pedido);
        }

        private Respuesta HazOperacion(Pedido pedido)
        {
            try
            {
                string mensaje = pedido.ToString();
                byte[] bufferTx = Encoding.UTF8.GetBytes(mensaje);

                flujo.Write(bufferTx, 0, bufferTx.Length);

                byte[] bufferRx = new byte[1024];
                int bytesRx = flujo.Read(bufferRx, 0, bufferRx.Length);

                string respuestaStr = Encoding.UTF8.GetString(bufferRx, 0, bytesRx);

                var partes = respuestaStr.Split(' ');
                return new Respuesta
                {
                    Estado = partes[0],
                    Mensaje = string.Join(" ", partes.Skip(1).ToArray())
                };
            }
            catch (SocketException ex)
            {

                throw new Exception("Error en la comunicación: " + ex.Message);
            }
        }
        public void CerrarConexion()
        {
            if (flujo != null)
                flujo.Close();
            if (cliente != null)
                cliente.Close();
        }
        /// Clase Protocolo: maneja la comunicación del cliente con el servidor.
        public class ProtocoloServidor
        {
            private Dictionary<string, int> listadoClientes;

            public ProtocoloServidor(Dictionary<string, int> clientes)
            {
                listadoClientes = clientes;
            }

            public Respuesta ResolverPedido(Pedido pedido, string direccionCliente)
            {
                Respuesta respuesta = new Respuesta
                { Estado = "NOK", Mensaje = "Comando no reconocido" };

                switch (pedido.Comando)
                {
                    case "INGRESO":
                        if (pedido.Parametros.Length == 2 &&
                            pedido.Parametros[0] == "root" &&
                            pedido.Parametros[1] == "admin20")
                        {
                            respuesta = new Random().Next(2) == 0
                                ? new Respuesta { Estado = "OK", Mensaje = "ACCESO_CONCEDIDO" }
                                : new Respuesta { Estado = "NOK", Mensaje = "ACCESO_NEGADO" };
                        }
                        else
                        {
                            respuesta.Mensaje = "ACCESO_NEGADO";
                        }
                        break;

                    case "CALCULO":
                        if (pedido.Parametros.Length == 3)
                        {
                            string placa = pedido.Parametros[2];
                            if (Regex.IsMatch(placa, @"^[A-Z]{3}[0-9]{4}$"))
                            {
                                byte indicadorDia = ObtenerIndicadorDia(placa);
                                respuesta = new Respuesta
                                { Estado = "OK", Mensaje = $"{placa} {indicadorDia}" };
                                ContadorCliente(direccionCliente);
                            }
                            else
                            {
                                respuesta.Mensaje = "Placa no válida";
                            }
                        }
                        break;

                    case "CONTADOR":
                        if (listadoClientes.ContainsKey(direccionCliente))
                        {
                            respuesta = new Respuesta
                            { Estado = "OK", Mensaje = listadoClientes[direccionCliente].ToString() };
                        }
                        else
                        {
                            respuesta.Mensaje = "No hay solicitudes previas";
                        }
                        break;
                }

                return respuesta;
            }

            private byte ObtenerIndicadorDia(string placa)
            {
                int ultimoDigito = int.Parse(placa.Substring(6, 1));
                switch (ultimoDigito)
                {
                    case 1: case 2: return 0b00100000; // Lunes
                    case 3: case 4: return 0b00010000; // Martes
                    case 5: case 6: return 0b00001000; // Miércoles
                    case 7: case 8: return 0b00000100; // Jueves
                    case 9: case 0: return 0b00000010; // Viernes
                    default: return 0;
                }
            }

            private void ContadorCliente(string direccionCliente)
            {
                if (listadoClientes.ContainsKey(direccionCliente))
                    listadoClientes[direccionCliente]++;
                else
                    listadoClientes[direccionCliente] = 1;
            }

            
        }

    }
}
