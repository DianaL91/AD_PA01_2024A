// **************************
// Práctica 07
// Diana Rodriguez 
// Fecha de realización: 26/11/2025
// Fecha de entrega: 03/12/2025
// Resultados:
//   * Se utilizó GitHub para la gestión de versiones del proyecto,
//     aplicando comandos básicos como clone, commit, push y pull.
//   * Se clonó el repositorio desde GitHub utilizando la URL proporcionada,
//     asegurando que el proyecto local estuviera sincronizado con la versión
//     remota. Una vez clonado, se realizaron modificaciones en el código
//     existentes para mejorar la estructura y legibilidad:
//       - Se añadieron comentarios explicativos en las clases y métodos,
//         describiendo su propósito y funcionamiento.
//   * Se modificó el proyecto para incorporar una nueva clase Protocolo,
//     la cual centraliza la lógica de comunicación entre cliente y servidor,
//     haciendo uso de las clases Pedido y Respuesta.
//   * Se actualizaron las clases Cliente y Servidor para que los métodos
//     HazOperación y ResolverPedido sean implementados en la clase Protocolo,
//     eliminando la dependencia directa de Pedido y Respuesta.
// Conclusiones:
//   * GitHub es una herramienta fundamental para la gestión de versiones,
//     permitiendo mantener un historial claro de cambios y facilitar la
//     colaboración en equipo.
//   * La integración de la clase Protocolo mejoró la modularidad y la
//     organización del código, centralizando la lógica del protocolo.
//   * El uso de Visual Studio junto con GitHub simplifica el flujo de trabajo
//     para proyectos en equipo.
//   * Resolver problemas en clases existentes refuerza la importancia de
//     pruebas y revisiones continuas durante el desarrollo.
// Recomendaciones:
//   * Mantener una estructura clara en los commits, utilizando mensajes
//     descriptivos para facilitar el seguimiento de cambios.
//   * Implementar pruebas unitarias para validar el correcto funcionamiento
//     de la clase Protocolo y sus métodos.
//   * Considerar la creación de ramas (branches) para nuevas funcionalidades,
//     evitando afectar la rama principal durante el desarrollo.
//   * Explorar herramientas de integración continua (CI) para automatizar
//     compilación y pruebas en futuros proyectos.
// **************************
using Protocolo;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using static Protocolo.Protocolo;

namespace Servidor
{
    class Servidor
    {
        private static TcpListener escuchador;
        private static Dictionary<string, int> listadoClientes
            = new Dictionary<string, int>();
        static void Main(string[] args)
        {
            try
            {
                escuchador = new TcpListener(IPAddress.Any, 8080);
                escuchador.Start();
                Console.WriteLine("Servidor inició en el puerto 8080...");

                while (true)
                {
                    TcpClient cliente = escuchador.AcceptTcpClient();
                    Console.WriteLine("Cliente conectado, puerto: {0}", cliente.Client.RemoteEndPoint.ToString());
                    Thread hiloCliente = new Thread(ManipuladorCliente);
                    hiloCliente.Start(cliente);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Error de socket al iniciar el servidor: " +
                    ex.Message);
            }
            finally 
            {
                escuchador?.Stop();
            }
        }

        private static void ManipuladorCliente(object obj)
        {
            TcpClient cliente = (TcpClient)obj;
            NetworkStream flujo = null;
            try
            {
                flujo = cliente.GetStream();
                byte[] bufferTx;
                byte[] bufferRx = new byte[1024];
                int bytesRx;
                ProtocoloServidor protocoloServidor = new ProtocoloServidor(listadoClientes);
                while ((bytesRx = flujo.Read(bufferRx, 0, bufferRx.Length)) > 0)
                {
                    string mensajeRx =
                        Encoding.UTF8.GetString(bufferRx, 0, bytesRx);
                    Pedido pedido = Pedido.Procesar(mensajeRx);
                    Console.WriteLine("Se recibio: " + pedido);

                    string direccionCliente =
                        cliente.Client.RemoteEndPoint.ToString();

                    // Usar la instancia para resolver el pedido
                    Respuesta respuesta = protocoloServidor.ResolverPedido(pedido, direccionCliente);
                    Console.WriteLine("Se envió: " + respuesta);

                    bufferTx = Encoding.UTF8.GetBytes(respuesta.ToString());
                    flujo.Write(bufferTx, 0, bufferTx.Length);
                }

            }
            catch (SocketException ex)
            {
                Console.WriteLine("Error de socket al manejar el cliente: " + ex.Message);
            }
            finally
            {
                if (flujo != null)
                    flujo.Close();
                if (cliente != null)
                    cliente.Close();
            }
        }
    }
}
