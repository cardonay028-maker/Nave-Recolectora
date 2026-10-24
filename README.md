# Nave Recolectora

Tarea individual de Unity: Scripting tradicional vs Visual Scripting
Hecho por: Yair Cardona

![juego](Docs/img/vista_juego.png)

## De qué trata el juego

Es un juego 2D sencillo. Controlas una nave en el espacio con las flechas del teclado y tienes que recoger los 5 cristales que hay en la pantalla. Cada vez que recoges uno suena un sonidito y el contador de arriba sube ("Cristales: 1", "Cristales: 2"...). Cuando juntas los 5 sale el mensaje de "¡MISIÓN COMPLETA!" con una musiquita de victoria.

Como pedía la tarea lo dividí en dos partes:

- El **movimiento** de la nave lo hice con un script en **C#** (`NaveMovimiento.cs`).
- La **recolección** de los cristales la hice con **Visual Scripting** usando un Script Machine en la nave.

Lo hice en Unity 6 con la plantilla 2D.

## Movimiento con C#

El script está en `Assets/NaveRecolectora/Scripts/NaveMovimiento.cs`.

Lo que hace:

- En el `Update` leo las flechas y armo un vector con la dirección (izquierda/derecha en X y arriba/abajo en Y).
- En el `FixedUpdate` muevo la nave. Para que el movimiento sea suave no le pongo la velocidad de golpe, uso `Vector2.MoveTowards` para que vaya acelerando poco a poco y cuando suelto las teclas vaya frenando poco a poco. Puse el frenado más bajo que la aceleración para que la nave se deslice un poquito, como si flotara en el espacio.
- Limité la posición con `Mathf.Clamp` usando el tamaño de la cámara para que la nave no se salga de la pantalla.
- También le puse que se incline un poco hacia el lado al que va, solo para que se vea mejor.
- Uso `rb.MovePosition` del Rigidbody2D en vez de mover el transform directamente, porque así la nave sigue detectando los triggers de los cristales.

Las variables `velocidad`, `aceleracion`, `frenado`, `inclinacion` y `margen` son públicas, así que se pueden cambiar desde el Inspector para probar.

## Recolección con Visual Scripting

En la nave agregué un componente **Script Machine** con un grafo que se llama `RecolectarCristales`. También creé estas variables de objeto en la nave:

- `cristales` (número, empieza en 0)
- `textoCristales` (el texto del contador)
- `panelVictoria` (el mensaje de victoria, empieza desactivado)
- `sonidoCristal` y `sonidoVictoria` (los sonidos)

Lo dividí en dos grupos:

**Recolección**

![grafo recoleccion](Docs/img/grafo_recoleccion.png)

1. `On Trigger Enter 2D` se activa cuando la nave toca algo.
2. Con `Get Game Object` saco el objeto que tocó y con `Compare Tag` reviso si tiene el tag "Cristal". Eso va a un `If`.
3. Si es un cristal, uso `Get Variable` + `Add` para sumarle 1 a `cristales` y lo guardo con `Set Variable`.
4. Con `Destroy` borro el cristal.
5. Con `String Concat` junto el texto "Cristales: " con el número y lo pongo en pantalla con `Set Text`.
6. Con `Play One Shot` suena el sonido del cristal.

**Ganar**

![grafo ganar](Docs/img/grafo_ganar.png)

1. Con `Equal` comparo si `cristales` ya es 5 y lo mando a un `If`.
2. Si es verdad, con `Set Active` muestro el panel de victoria y con otro `Play One Shot` suena la música de victoria.

Los cristales tienen un `CircleCollider2D` con **Is Trigger** activado y el tag "Cristal". La nave tiene un `Rigidbody2D` (con gravedad en 0) y un `CircleCollider2D` normal.

## Problemas que tuve y cómo los solucioné

- **La nave se iba para abajo.** Era la gravedad del Rigidbody2D, así que puse `gravityScale = 0` porque en el espacio no hay gravedad.
- **En diagonal iba más rápido.** Al presionar dos flechas el vector medía más que 1. Lo arreglé con `direccion.normalized`.
- **El movimiento era muy brusco.** Al principio arrancaba y frenaba de golpe. Lo cambié por `MoveTowards` con aceleración y frenado.
- **Error con `Input.GetKey`.** En Unity 6 el proyecto usa el Input System nuevo y me salía un error, así que leí las teclas con `Keyboard.current`.
- **La nave se salía de la pantalla.** Usé `Mathf.Clamp` con el tamaño de la cámara.
- **El `On Trigger Enter 2D` no funcionaba.** Me faltaba marcar **Is Trigger** en el collider del cristal. Además hay que usar el evento 2D y no el normal, porque el juego es 2D.
- **Se sumaban puntos al tocar otras cosas.** Le agregué el `Compare Tag` para que solo cuente los que tienen el tag "Cristal".
- **El contador mostraba el número anterior.** Lo arreglé usando el valor que sale del `Set Variable` (que ya está sumado) para el texto y para comparar con 5.
- **Salió un aviso de Visual Scripting en Play Mode.** Unity me avisó que recargar scripts mientras juegas da problemas con Visual Scripting, así que le di a "Change now" para que pare y recompile.

## Ventajas y desventajas de cada forma

**Facilidad de implementación**

Con C# hay que saberse la sintaxis y las funciones de Unity, y si escribes algo mal no compila. Pero para el movimiento fue lo más fácil porque son cálculos con vectores, y en pocas líneas pude hacer la aceleración, los límites y la inclinación. Hacer eso con nodos habría sido un montón de bloques.

Con Visual Scripting fue más fácil empezar porque solo buscas el nodo y lo conectas, no hay errores de escritura. Para la recolección fue rápido porque es algo de eventos: toca, revisa, suma, borra. Lo malo es que algo simple como sumar 1 necesita 3 nodos (Get, Add y Set).

**Legibilidad**

El código en C# es más compacto, todo cabe en una pantalla y con los comentarios se entiende bien, aunque para alguien que no programa puede ser difícil de leer.

El grafo se entiende muy rápido cuando es pequeño, porque ves las flechas y sabes qué pasa después de qué. Pero si fuera más grande se llenaría de cables cruzados y sería difícil seguirlo. Por eso lo ordené en grupos.

**Escalabilidad**

C# escala mucho mejor: puedes hacer más clases, reutilizar código y trabajar en equipo con GitHub viendo exactamente qué línea cambió cada uno.

Con Visual Scripting los grafos se guardan como archivos que no se pueden comparar bien en GitHub, y si el proyecto crece se vuelve más complicado de mantener. También es un poco más lento que el código, así que no conviene para cosas que se ejecutan en cada frame, como el movimiento.

**Conclusión**

Para este juego cada forma sirvió para algo diferente. El movimiento quedó mejor en C# porque se ejecuta siempre y tiene cálculos, y la recolección quedó bien en Visual Scripting porque es lógica de eventos y se ve clarito en el grafo. Si hiciera un juego más grande usaría C# para lo principal y Visual Scripting solo para cosas puntuales o para probar ideas rápido.

## Archivos

```
Assets/NaveRecolectora/Scripts/NaveMovimiento.cs   -> script de movimiento
Docs/img/grafo_recoleccion.png                     -> grafo de la recolección
Docs/img/grafo_ganar.png                           -> grafo de cuando ganas
Docs/img/vista_juego.png                           -> captura del juego
```
