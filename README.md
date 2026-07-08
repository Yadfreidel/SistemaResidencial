# Sistema de Gestión Residencial (Arquitectura MVVM)

Este es el repositorio oficial del Sistema de Gestión Residencial. Es una aplicación de escritorio diseñada para administrar inquilinos, apartamentos, contratos y pagos en un entorno de alquiler. La prioridad de este proyecto fue crear un Producto Mínimo Viable (MVP) robusto que aguante operaciones pesadas sin colapsar.

## Arquitectura del Sistema

El código base original era un desastre de mantenimiento. Para detener el caos operativo y evitar el código espagueti, estructuramos el proyecto separando el flujo de datos con MVC y estabilizando la interfaz gráfica con el patrón **MVVM (Modelo-Vista-ViewModel)**.

### El Enfoque MVVM

Meter consultas a la base de datos y validaciones en los eventos de los botones congela la pantalla. Al implementar MVVM, dividimos el problema:

1. **Modelos:** Entidades puras (Inquilino, Pago). No saben que existe una interfaz ni una base de datos.
2. **ViewModels (El Motor Reactivo):** Funcionan como despachadores asíncronos. Procesan la lógica y exponen las propiedades observables mediante la interfaz `INotifyPropertyChanged`.
3. **Vistas (UI):** Los formularios son un simple cascarón visual. Se enlazan a los ViewModels mediante *Data Binding*. El usuario presiona un botón, el ViewModel cambia el estado por detrás y la pantalla reacciona sola.

Nos quitamos de encima la manipulación manual de los componentes de Windows y las pantallas frisadas.

## Estructura de Directorios Real

Así estructuramos las capas para evitar acoplamiento:

```text
SISTEMA/
├── Vistas/        (Formularios UI interactivos enlazados por Data Binding)
├── Controllers/   (Enrutamiento de peticiones)
├── Repositories/  (Consultas SQL encapsuladas)
├── Interfaces/    (Contratos para inyección de dependencias)
├── Models/        (Entidades de base de datos)
└── Data/          (Configuración de la conexión)
```

## Beneficios Prácticos del Diseño

Optamos por esta arquitectura por puro pragmatismo. Consumimos un poco más de memoria para mantener los bindings de MVVM activos, pero ganamos estabilidad total. Si mañana cambia una regla de negocio o una tabla, tocamos los repositorios o el ViewModel. La vista visual ni se entera y el usuario final no sufre errores en pantalla.

## Despliegue y Ejecución

1. Descarga o clona el repositorio.
2. Configura la cadena de conexión en tu capa `Data` para apuntar al servidor local.
3. Compila la solución. La inyección de dependencias se encarga de instanciar los repositorios y pasarlos a los formularios. El sistema levanta de inmediato.

---

**Arquitectura y estructuración por:** Luis Martín Peña Mejía.
