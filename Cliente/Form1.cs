using System;
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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using Protocolo;

namespace Cliente
{
    public partial class FrmValidador : Form
    {
        private Protocolo.Protocolo protocolo;

        public FrmValidador()
        {
            InitializeComponent();
        }

        private void FrmValidador_Load(object sender, EventArgs e)
        {
            try
            {
                protocolo = new Protocolo.Protocolo("127.0.0.1", 8080);
            }
            catch (SocketException ex)
            {
                MessageBox.Show("No se puedo establecer conexión " + ex.Message,
                    "ERROR");
            }
    
            panPlaca.Enabled = false;
            chkLunes.Enabled = false;
            chkMartes.Enabled = false;
            chkMiercoles.Enabled = false;
            chkJueves.Enabled = false;
            chkViernes.Enabled = false;
            chkDomingo.Enabled = false;
            chkSabado.Enabled = false;
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text;
            string contraseña = txtPassword.Text;
            if (usuario == "" || contraseña == "")
            {
                MessageBox.Show("Se requiere el ingreso de usuario y contraseña",
                    "ADVERTENCIA");
                return;
            }

            Pedido pedido = new Pedido
            {
                Comando = "INGRESO",
                Parametros = new[] { usuario, contraseña }
            };
            
            Respuesta respuesta = protocolo.ResolverPedido(pedido);
            if (respuesta == null)
            {
                MessageBox.Show("Hubo un error", "ERROR");
                return;
            }

            if (respuesta.Estado == "OK" && respuesta.Mensaje == "ACCESO_CONCEDIDO")
            {
                panPlaca.Enabled = true;
                panLogin.Enabled = false;
                MessageBox.Show("Acceso concedido", "INFORMACIÓN");
                txtModelo.Focus();
            }
            else if (respuesta.Estado == "NOK" && respuesta.Mensaje == "ACCESO_NEGADO")
            {
                panPlaca.Enabled = false;
                panLogin.Enabled = true;
                MessageBox.Show("No se pudo ingresar, revise credenciales",
                    "ERROR");
                txtUsuario.Focus();
            }
        }

        
        private void btnConsultar_Click(object sender, EventArgs e)
        {
            string modelo = txtModelo.Text;
            string marca = txtMarca.Text;
            string placa = txtPlaca.Text;
            
            Pedido pedido = new Pedido
            {
                Comando = "CALCULO",
                Parametros = new[] { modelo, marca, placa }
            };
            
            Respuesta respuesta = protocolo.ResolverPedido(pedido);
            if (respuesta == null)
            {
                MessageBox.Show("Hubo un error", "ERROR");
                return;
            }

            if (respuesta.Estado == "NOK")
            {
                MessageBox.Show("Error en la solicitud.", "ERROR");
                chkLunes.Checked = false;
                chkMartes.Checked = false;
                chkMiercoles.Checked = false;
                chkJueves.Checked = false;
                chkViernes.Checked = false;
            }
            else
            {
                var partes = respuesta.Mensaje.Split(' ');
                MessageBox.Show("Se recibió: " + respuesta.Mensaje,
                    "INFORMACIÓN");
                byte resultado = Byte.Parse(partes[1]);
                switch (resultado)
                {
                    case 0b00100000:
                        chkLunes.Checked = true;
                        chkMartes.Checked = false;
                        chkMiercoles.Checked = false;
                        chkJueves.Checked = false;
                        chkViernes.Checked = false;
                        break;
                    case 0b00010000:
                        chkMartes.Checked = true;
                        chkLunes.Checked = false;
                        chkMiercoles.Checked = false;
                        chkJueves.Checked = false;
                        chkViernes.Checked = false;
                        break;
                    case 0b00001000:
                        chkMiercoles.Checked = true;
                        chkLunes.Checked = false;
                        chkMartes.Checked = false;
                        chkJueves.Checked = false;
                        chkViernes.Checked = false;
                        break;
                    case 0b00000100:
                        chkJueves.Checked = true;
                        chkLunes.Checked = false;
                        chkMartes.Checked = false;
                        chkMiercoles.Checked = false;
                        chkViernes.Checked = false;
                        break;
                    case 0b00000010:
                        chkViernes.Checked = true;
                        chkLunes.Checked = false;
                        chkMartes.Checked = false;
                        chkMiercoles.Checked = false;
                        chkJueves.Checked = false;
                        break;
                    default:
                        chkLunes.Checked = false;
                        chkMartes.Checked = false;
                        chkMiercoles.Checked = false;
                        chkJueves.Checked = false;
                        chkViernes.Checked = false;
                        break;
                }
            }
        }

        private void btnNumConsultas_Click(object sender, EventArgs e)
        {
            String mensaje = "hola";
            
            Pedido pedido = new Pedido
            {
                Comando = "CONTADOR",
                Parametros = new[] { mensaje }
            };

            Respuesta respuesta = protocolo.ResolverPedido(pedido);
            if (respuesta == null)
            {
                MessageBox.Show("Hubo un error", "ERROR");
                return;
            }

            if (respuesta.Estado == "NOK")
            {
                MessageBox.Show("Error en la solicitud.", "ERROR");

            }
            else
            {
                var partes = respuesta.Mensaje.Split(' ');
                MessageBox.Show("El número de pedidos recibidos en este cliente es " + partes[0],
                    "INFORMACIÓN");
            }
        }

        private void FrmValidador_FormClosing(object sender, FormClosingEventArgs e)
        {
            protocolo.CerrarConexion();
        }
    }
}
