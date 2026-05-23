# Inteligencia Artificial con Unity

## Trabajo Práctico 2

La entrega deberá consistir en un link a un repositorio de GitHub en donde se encuentre alojado un proyecto de Unity. Si bien puede ser desarrollado en cualquier versión del editor que se prefiera, la entrega debe poder ser abierta desde Unity 6 (versión 6000.0.36f1) y seguir funcionando debidamente. Asimismo, debe evidenciarse que el proyecto fue elaborado usando versionado (debe tener varios commits, a contraposición de uno solo con la consigna completada).

## Ejercicio

El proyecto debe consistir en un juego **top-down shooter survival endless**. El jugador posee un arma de fuego que debe usar para dispararle a los distintos tipos de enemigos. Según el tipo al que pertenezca, cada enemigo se comporta de manera distinta (mediada por su propia máquina de estados). Los enemigos deben moverse por un mapa que presente distintos obstáculos, usando un sistema de pathfinding. La idea es acumular puntaje con cada enemigo eliminado. La única forma de perder es cuando el jugador pierde toda la salud.

## Criterios de evaluación

Se dispone de un sistema de tiers, donde cada uno dispone de requisitos a cumplir para ser alcanzado. Contar con uno de los requisitos de un tier otorgará un punto. Con la finalidad de pasar de un tier a otro, es necesario cumplir con todos los requisitos indicados en él. Es decir, no se considerarán requisitos de un determinado tier si los pertenecientes al anterior no están todos logrados.

> Tecnicatura Superior en Programación de Videojuegos con Motores

### Tier B: 4 (cuatro)

- Los agentes se mueven por el mapa usando el **NavMesh** de Unity.
- Los enemigos distribuyen la lógica de sus estados mediante **State Machine Behaviours** vinculados a un Animator de Unity.
- Existe un arquetipo de enemigo **melee** que presenta al menos tres estados (patrulla, persecución y ataque); su comportamiento lo hace acercarse al jugador para atacar, y solamente puede atacar cuando está dentro del rango mínimo.
- Existe un arquetipo de enemigo **ranged** que presente al menos tres estados (patrulla, persecución y ataque); su comportamiento lo hace acercarse hasta cierta distancia al jugador antes de disparar, pero se aleja si el jugador pasa a estar muy cerca.

### Tier A: 7 (siete)

- Existen variantes de enemigos **rápidos y lentos**, donde los más rápidos son los únicos que pueden saltar por zonas de NavMesh unidas por **links**.
- Existen **zonas de mayor costo** que hacen que cualquiera que pase por ellas (tanto jugador como enemigo) se mueva más lentamente.
- Existe un tipo de agente que consiste en un **NPC**, el cual puede ser atacado por los enemigos; si es salvado antes de que muera, le deja una bonificación al jugador (ya sean puntos o un power up).

### Tier S: 10 (diez)

- El juego presenta una mecánica de **recarga de munición**, haciendo que el jugador tenga un botón para recargar, además de tener que recorrer el mapa en búsqueda de más munición.
- Los enemigos se vuelven **más agresivos** cuando el jugador tiene poca salud.
- Existe un tipo de enemigo **elite** que posee distintas "fases"; con cada una de ellas, presenta un patrón de movimiento y ataque distinto al de cualquier otro enemigo (a libre elección la definición de cómo se mueve y cómo ataca).
