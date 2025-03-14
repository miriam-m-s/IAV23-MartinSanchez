# IAV23_MartínSánchez_RolSánchez
# IAV - Base para la Práctica 1

[VIDEO PRUEBAS](https://youtu.be/rU06EAn3T9E)
- [Miriam Martin Sanchez](https://github.com/miriam-m-s)

## Propuesta


Esta práctica consiste en desarrollar un prototipo de IA para Videojuegos, dentro de un entorno virtual con obstáculos y enemigos y con un avatar en primera persona controlado por el jugador. Dicho entorno contará con una generación procedural por lo que será distinto en cada partida.

El **enemigo**(MIRIAM MARTÍN SÁNCHEZ) tendrá cinco estados:
* **PERSECUCIÓN** : cuando el avatar jugador este en su rango de vista pero no este a tiro, el enemigo le perseguira evitando paredes...
* **MERODEO**: cuando el jugador no este en su rango de visión,el enemigo merodeará en las salas.
* **PERCEPCIÓN**: el enemigo mientras merodea si percibe al jugador detras de él,se girara para comenzará el ataque.
* **ATAQUE**: cuando el juagdor este a tiro  este apuntará al jugador y lo disparará.
* **MUERTE** :cuando el enmigo se queda sin vida este morirá.





## Diseño de la solución
### ENEMIGO
ESTADOS DE LOS ENEMIGIGOS
<p align="center">
 <img src="https://github.com/miriam-m-s/IAV23-MartinSanchez/blob/main/images/flujo.jpeg" width="500"/></td>
</p>

### MERODEAR
El enemigo comenzará merodenado,para ello utilizaremos como punto de partida el codigo de Millington.</br>
El pseudocódigo del algoritmo de merodear es:

```python
class KinematicWander:

    character: Static
    maxSpeed: float
    
    timer: float    # 
    maxTime: float  # how many seconds before selecting a new direction

    # The maximum rotation speed we’d like, probably should be smaller
    # than the maximum possible, for a leisurely change in direction.
    maxRotation: float
    
    function getSteering() -> KinematicSteeringOutput:
        result = new KinematicSteeringOutput()

        # Get velocity from the vector form of the orientation.
        result.velocity = maxSpeed * character.orientation.asVector()

        if timer > maxTime:
            # Change our orientation randomly.
            result.rotation = randomBinomial() * maxRotation
            timer = 0;
        
        else:
            timer += Time.deltaTime

     return result
```
(Pag 76.AI for Games)</br>
Para que el merodeo sea más realista, el enemigo pausará y realizará su animación de inactividad (idle). Esto significa que cuando el enemigo esté en modo de merodeo, dejará de moverse activamente y realizará una animación que indica que está en reposo.

Además, durante el merodeo, el enemigo evitará chocar con las paredes. Para lograrlo, el enemigo lanzará un rayo (raycast) en la dirección de su movimiento para detectar si hay una pared en su camino. Si el rayo detecta una pared a una distancia cercana, el enemigo tomará medidas para evitarla.

Para evitar la colisión con la pared, el enemigo calculará una nueva rotación y posición basándose en la normal (dirección perpendicular) del plano de la pared detectada. Esto significa que el enemigo ajustará su dirección y posición para rodear la pared y continuar su merodeo sin chocar.

Estas mejoras ayudarán a que el comportamiento del enemigo durante el merodeo sea más realista y a evitar colisiones con las pared.
### CONO DE VISIÓN


```python
class ConeVision:
  # Define the position and direction of the observer
  observer_position = (x, y)
  observer_direction = (dx, dy)

  # Define the cone parameters
  cone_angle = 45  # Angle in degrees
  cone_distance = 10  # Maximum distance of visibility

  # Iterate through all objects in the scene
  for object in scene_objects:
      object_position = object.get_position()

      # Calculate the vector from the observer to the object
      dx = object_position[0] - observer_position[0]
      dy = object_position[1] - observer_position[1]

      # Calculate the angle between the observer's direction and the object
      angle = math.atan2(dy, dx)  # Angle in radians
      angle_degrees = math.degrees(angle)  # Convert to degrees

      # Check if the object is within the cone of vision
      if abs(angle_degrees - observer_direction) <= cone_angle / 2 and math.hypot(dx, dy) <= cone_distance:
          # Object is within the cone of vision
          # Do something with the object
          object.process()
```
El cono de visión se utilizará para la vista del enemigo y para su percepción.n El cono de visión se utilizráa para describir el rango de visibilidad o alcance del enemigo.La porción del espacio del enemigo que podra ver y asi procesar esa información.

Los elementos visibles que podrá procesar nuestro enemigo será las paredes y el jugador. 


Para lograr esto, implementaremos un algoritmo similar al pseudocódigo. Se utilizará un raycasts lanzados desde la posición enemigo, y la cantidad y dirección de estos rayos dependerán de la resolución deseada. Cada rayo tendrá un rango de distancia y un ángulo, que determinarán el tamaño y la forma del cono de visión. Estos rayos detectarán tanto al jugador como a las paredes.

Durante la ejecución del algoritmo, el enemigo lanzará los rayos en diferentes direcciones dentro del rango de ángulo especificado. Para cada rayo, se verificará si hay colisión con el jugador o con una pared. Si se detecta una colisión con el jugador, el enemigo tomará una decisión.

Cuanta mayor sea la resolución mayor será la cantidad de rayos, mayor será la precisión en la detección de objetos y paredes, pero también requerirá más poder de procesamiento.

Implementar este enfoque permitirá que el enemigo tenga un cono de visión que pueda detectar tanto al jugador como a las paredes, brindando una interacción más realista.




#### VISION ENEMIGO
<p align="center">
 <img src="https://github.com/miriam-m-s/IAV23-MartinSanchez/blob/main/images/conovision.jpg" width="500"/></td>
</p>
El enemigo cuenta con un cono de visión que le permite detectar al jugador. Dependiendo de la situación, el enemigo puede entrar en dos estados distintos:

* **Ataque:** Si la distancia entre el jugador y el enemigo es igual o menor que el rango de ataque predefinido, el enemigo iniciará el estado de ataque. En este estado, el enemigo realizará su animación de disparo y rotará hacia la posición actual del jugador, preparándose para lanzar ataques.

* **Persecución:** Si la distancia entre el jugador y el enemigo es mayor que el rango de ataque, el enemigo entrará en el estado de persecución. En este estado, el enemigo perseguirá activamente al jugador, moviéndose hacia su posición con el objetivo de acercarse lo suficiente para realizar un ataque.

Estos estados permiten al enemigo adaptar su comportamiento según la distancia entre él y el jugador. Si el jugador está dentro del rango de ataque, el enemigo tomará medidas ofensivas y tratará de eliminar al jugador. Si el jugador se encuentra fuera del rango de ataque, el enemigo se centrará en perseguir al jugador hasta que esté lo suficientemente cerca para lanzar un ataque efectivo.





### PERSECUCIÓN

El pseudocódigo del algoritmo de movimiento de persecución es:
```python
class Pursue extends Seek:
# The maximum prediction time.
maxPrediction: float

# OVERRIDES the target data in seek (in other words this class has
# two bits of data called target: Seek.target is the superclass
# target which will be automatically calculated and shouldn’t be
# set, and Pursue.target is the target we’re pursuing).
target: Kinematic

 # ... Other data is derived from the superclass ...

 function getSteering() -> SteeringOutput:
 # 1. Calculate the target to delegate to seek
 # Work out the distance to target.
 direction = target.position - character.position
 distance = direction.length()

 # Work out our current speed.
 speed = character.velocity.length()

 # Check if speed gives a reasonable prediction time.
 if speed <= distance / maxPrediction:
 prediction = maxPrediction

 # Otherwise calculate the prediction time.
 else:
 prediction = distance / speed

 # Put the target together.
 Seek.target = explicitTarget
 Seek.target.position += target.velocity * prediction

 # 2. Delegate to seek.
 return Seek.getSteering()
```
(Pag 89.AI for Games)

#### PERCEPCIÓN
<p align="center">
 <img src="https://github.com/miriam-m-s/IAV23-MartinSanchez/blob/main/images/cono.jpg" width="500"/></td>
</p>

El enemigo tiene la capacidad de percibir al jugador, incluso con un rango y ángulo más reducidos, por ejemplo, detectándolo incluso si se encuentra detrás del enemigo. El enemigo tiene un cono de visión que se extiende hacia detras (-vector.forward) y tiene un rango inferior al de vista pero su angulo es la diferencia del total con el ángulo de percepción. Cuando el jugador es detectado dentro del cono de visión, se muestra una imagen de interrogación sobre la cabeza del enemigo y se activa un temporizador aleatorio entre 2 y 5 segundos. Si el jugador permanece dentro del rango de percepción del enemigo durante ese tiempo, el enemigo girará para mirarlo directamente y entrará en modo de ataque.
## Implementacion
<p align="center">
 <img src="https://github.com/miriam-m-s/IAV23-MartinSanchez/blob/main/images/stateunity.jpeg" width="500"/></td>
</p>
Para el comportamiento del enemigo se ha implementado un sistema de control basado en una máquina de estados utilizando Visual Scripting. La visión del enemigo y su percepción determinarán las transiciones entre los diferentes estados de la máquina.</br>

Cuando el enemigo no pueda detectar al jugador, seguirá una rutina predefinida en la que merodeará por toda la sala. Sin embargo, si el enemigo visualiza o percibe al jugador, activará otras acciones como atacar y perseguir al objetivo.

## Pruebas y métricas
Maquina de estados 
* A: Enemigo te percibe si estas detras de él.
* B: vista del enemigo
* C: merodeo
* D : eataque y persecución
* E: muerte
En el video se explica detalladamente todos estos apartados </br>
[VIDEO PRUEBAS](https://youtu.be/rU06EAn3T9E)

## Ampliaciones

- Estética del proyecto mejorada 
<p align="center">
 <img src="https://github.com/miriam-m-s/IAV23-MartinSanchez/blob/main/images/enemy.jpg" width="500"/></td>
 </p>
 Se ha riggeado el personaje y se ha metido animaciones de mixamo mediante blender para luego ser exportado a unity.</br>
 Se ha realizado una maquina de estados para activar y desactivar animaciones del personaje.</br>

- Sistema de particulas:</br>
 Se ha diseñado un sistema de particulas acorde con la estetcia del proyecto para que cuando dispares al enemigo este sistema se genere.
 
 <p align="center">
 <img src="https://github.com/miriam-m-s/IAV23-MartinSanchez/blob/main/images/shoot.jpg" width="500"/></td>
 </p>

- Interfaz</br>
Se ha añadido interfaz al jugador y al enemigo.

## Producción

Las tareas se han realizado y el esfuerzo ha sido repartido entre los autores.

| Estado  |  Tarea  |  Fecha  |  
|:-:|:--|:-:|
| ✔ | Readme | 26-05-2023 |
| ✔ | ataque | 10-5-2023 |
| ✔ | Merodear| 22-5-2023|
| ✔ | Persecución| 10-5-2023 |
| ✔ | Vista Enemigo| 12-5-2023 |
| ✔ | Refactorizacion código| 26-5-2023 |
| ✔ | muerte| 16-5-2023 |
|   | ... | |
|  | OPCIONAL |  |
| ✔ | Animaciones | 16-5-2023  |
| ✔ | Particulas |16-5-2023  |
| ✔ | VFX | 16-5-2023  |
| ✔ | Interfaz | 16-5-2023 |
| ✔ | Estetica | 16-5-2023  |

 #  [PROPUESTA DIEGO ROL SÁNCHEZ](https://github.com/DiegoRol69/IAV23-RolS-nchez)

## Referencias

Los recursos de terceros utilizados son de uso público.

- *AI for Games*, Ian Millington.

