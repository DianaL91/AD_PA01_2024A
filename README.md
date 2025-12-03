📌 Proyecto: Prueba Acumulativa 01
Repositorio: https://github.com/DianaL91/AD_PA01_2024A

✅ Objetivos

Implementar un sistema cliente-servidor con protocolo propio.
Gestionar versiones del proyecto usando GitHub.
Aplicar principios de modularidad y reutilización en la arquitectura del sistema.


🔍 Cambios realizados

Se agregó la clase Protocolo para centralizar la lógica de comunicación.
Se modificaron las clases Cliente y Servidor para usar la clase Protocolo en lugar de Pedido y Respuesta.
- Se corrigió el error en la clase Servidor: la lógica de procesamiento se trasladó a la clase Protocolo, eliminando el acoplamiento directo con Pedido y Respuesta y mejorando la modularidad.
Se añadieron encabezados y comentarios explicativos en el código para mejorar la documentación.


📂 Estructura del proyecto
Cliente/
Protocolo/
Servidor/
PruebaAcumulativa01_2024A.sln
README.md
