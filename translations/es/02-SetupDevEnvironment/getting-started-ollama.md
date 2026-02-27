# Configurar el Entorno de Desarrollo con Ollama

Si deseas usar Ollama para ejecutar modelos locales en este curso, sigue los pasos de esta guía.

¿No quieres usar Azure OpenAI?

👉 [Si prefieres usar los Azure OpenAI, esta es la guía para ti](README.md)  
👉 [Aquí están los pasos para usar Ollama](getting-started-ollama.md)

## Crear un Codespace en GitHub

Vamos a crear un Codespace en GitHub para desarrollar durante el resto del curso.

1. Abre la página principal de este repositorio en una nueva ventana haciendo [clic derecho aquí](https://github.com/microsoft/Generative-AI-for-beginners-dotnet) y seleccionando **Abrir en una nueva ventana** en el menú contextual.  
1. Haz un fork de este repositorio en tu cuenta de GitHub haciendo clic en el botón **Fork** en la esquina superior derecha de la página.  
1. Haz clic en el botón desplegable **Code** y luego selecciona la pestaña **Codespaces**.  
1. Selecciona la opción **...** (los tres puntos) y elige **New with options...**  

![Crear un Codespace con opciones personalizadas](../../../translated_images/creating-codespace.0e7334f85cf4c8d0e080a0d5b4c76c24c5bbe6bddf48dcd1403e092ea0d9bce9.es.png)

### Elegir tu contenedor de desarrollo

En el menú desplegable **Dev container configuration**, selecciona una de las siguientes opciones:

**Opción 1: C# (.NET)**: Esta es la opción que debes usar si planeas utilizar los Azure OpenAI o Azure OpenAI. Es nuestra forma recomendada de completar este curso. Incluye todas las herramientas básicas de desarrollo .NET necesarias para el curso y tiene un tiempo de inicio rápido.

**Opción 2: C# (.NET) - Ollama**: Esta es la opción que necesitas si planeas ejecutar modelos localmente con Ollama. Incluye todas las herramientas básicas de desarrollo .NET además de Ollama, pero tiene un tiempo de inicio más lento, en promedio cinco minutos. [Sigue esta guía](getting-started-ollama.md) si deseas usar Ollama.

Puedes dejar el resto de las configuraciones como están. Haz clic en el botón **Create codespace** para iniciar el proceso de creación del Codespace.

![Seleccionar la configuración de tu contenedor de desarrollo](../../../translated_images/select-container-codespace.9b8ca34b6ff8b4cb80973924cbc1894cf7672d233b0055b47f702db60c4c6221.es.png)

## Verificar que tu Codespace funciona correctamente con Ollama

Una vez que tu Codespace esté completamente cargado y configurado, ejecutemos una aplicación de ejemplo para verificar que todo funcione correctamente:

1. Abre el terminal. Puedes abrir una ventana de terminal presionando **Ctrl+\`** (backtick) on Windows or **Cmd+`** en macOS.

1. Cambia al directorio correcto ejecutando el siguiente comando:

    ```bash
    cd 02-SetupDevEnvironment/src/BasicChat-03Ollama/
    ```

1. Luego, ejecuta la aplicación con el siguiente comando:

    ```bash
    dotnet run
    ```

1. Puede tardar unos segundos, pero eventualmente la aplicación debería mostrar un mensaje similar al siguiente:

    ```bash
    AI, or Artificial Intelligence, refers to the development of computer systems that can perform tasks that typically require human intelligence, such as:

    1. Learning: AI systems can learn from data and improve their performance over time.
    2. Reasoning: AI systems can draw conclusions and make decisions based on the data they have been trained on.
    
    ...
    ```

> 🙋 **¿Necesitas ayuda?**: ¿Algo no funciona? [Abre un issue](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new?template=Blank+issue) y te ayudaremos.

## Cambiar el modelo en Ollama

Una de las cosas geniales de Ollama es que es fácil cambiar de modelo. La aplicación actual utiliza el modelo "**llama3.2**". Cambiemos al modelo "**phi3.5**" para probarlo.

1. Descarga el modelo Phi3.5 ejecutando el siguiente comando en el terminal:

    ```bash
    ollama pull phi3.5
    ```

    Puedes obtener más información sobre el modelo [Phi3.5](https://ollama.com/library/phi3.5) y otros modelos disponibles en la [biblioteca de Ollama](https://ollama.com/library/).

1. Abre `02-SetupDevEnvironment/src/BasicChat-03Ollama/Program.cs` en tu editor.

    Actualiza la inicialización del cliente de chat para usar el nuevo modelo cambiando las líneas 6-7:

    ```csharp
    IChatClient client =
        new OllamaChatClient(new Uri("http://localhost:11434/"), "phi3.5");
    ```

1. Finalmente, ejecuta la aplicación con el siguiente comando:

    ```bash
    dotnet run
    ```

1. Acabas de cambiar a un nuevo modelo. Nota cómo la respuesta es más larga y detallada.

    ```bash
    Artificial Intelligence (AI) refers to the simulation of human intelligence processes by machines, especially computer systems. These processes include learning (the acquisition of information and accumulation of knowledge), reasoning (using the acquired knowledge to make deductions or decisions), and self-correction. AI can manifest in various forms:

    1. **Narrow AI** – Designed for specific tasks, such as facial recognition software, voice assistants like Siri or Alexa, autonomous vehicles, etc., which operate under a limited preprogrammed set of behaviors and rules but excel within their domain when compared to humans in these specialized areas.

    2. **General AI** – Capable of understanding, learning, and applying intelligence broadly across various domains like human beings do (natural language processing, problem-solving at a high level). General AIs are still largely theoretical as we haven't yet achieved this form to the extent necessary for practical applications beyond narrow tasks.
    
    ...
    ```

> 🙋 **¿Necesitas ayuda?**: ¿Algo no funciona? [Abre un issue](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new?template=Blank+issue) y te ayudaremos.

## Resumen

En esta lección, aprendiste a configurar tu entorno de desarrollo para el resto del curso. Creaste un Codespace en GitHub y lo configuraste para usar Ollama. También actualizaste el código de ejemplo para cambiar de modelo fácilmente.

### Recursos adicionales

- [Modelos de Ollama](https://ollama.com/search)  
- [Trabajar con Codespaces de GitHub](https://docs.github.com/en/codespaces/getting-started)  
- [Documentación de Microsoft Extensions for AI](https://learn.microsoft.com/dotnet/)  

## Próximos pasos

¡A continuación, exploraremos cómo crear tu primera aplicación de inteligencia artificial! 🚀

👉 [Técnicas fundamentales de Generative AI](../03-CoreGenerativeAITechniques/readme.md)

**Descargo de responsabilidad**:  
Este documento ha sido traducido utilizando servicios de traducción automática basados en inteligencia artificial. Si bien nos esforzamos por garantizar la precisión, tenga en cuenta que las traducciones automáticas pueden contener errores o inexactitudes. El documento original en su idioma nativo debe considerarse como la fuente autorizada. Para información crítica, se recomienda una traducción profesional realizada por humanos. No nos hacemos responsables de malentendidos o interpretaciones erróneas que puedan surgir del uso de esta traducción.