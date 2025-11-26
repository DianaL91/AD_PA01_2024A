using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Protocolo
{
    public class Pedido
    {
        public string Comando { get; set; }
        public string[] Parametros { get; set; }

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
    }

 }
