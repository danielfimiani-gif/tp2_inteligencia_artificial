# TP2 — Inteligencia Artificial

Top-down shooter survival endless desarrollado en **Unity 6 (6000.0.36f1)** para la materia Inteligencia Artificial.

El proyecto implementa los requisitos de los tres tiers de la consigna (B, A, S) usando NavMesh, State Machine Behaviours, Object Pooling y Singletons. Detalle completo del enunciado en [`Docs/consigna.md`](Docs/consigna.md).

---

## Requisitos previos

- **Unity 6 (6000.0.36f1)** o superior — instalable desde Unity Hub
- **Git** — https://git-scm.com
- **Git LFS** — https://git-lfs.com

> Este repositorio usa **Git LFS** para los binarios pesados (modelos `.fbx`, audios, texturas grandes). Si Git LFS no está instalado, `git clone` solo bajará archivos puntero de unos pocos bytes en lugar de los binarios reales y Unity tirará errores del estilo *"Couldn't read file"* o *"Required human bone 'Hips' not found"*.

---

## Cómo clonar el proyecto

1. Instalá Git LFS (una sola vez por máquina):

   ```bash
   git lfs install
   ```

2. Cloná el repo normalmente — los binarios se bajan automáticamente:

   ```bash
   git clone https://github.com/danielfimiani-gif/tp2_inteligencia_artificial.git
   ```

3. Abrí la carpeta desde **Unity Hub → Add project from disk** y seleccioná la versión `6000.0.36f1`.

### Si ya cloneaste sin Git LFS

No hace falta volver a clonar. Instalá Git LFS y desde la carpeta del proyecto corré:

```bash
git lfs install
git lfs pull
```

Esto reemplaza los punteros por los binarios reales. Después reabrí el proyecto en Unity y forzá un reimport de la carpeta `Assets/ThirpartyAssets` si Unity no detecta los cambios solo.

---

## Cómo correr el juego

1. Abrí la escena `Assets/_Project/Scenes/MainMenu.unity`.
2. Play en el editor.
3. Click en **Play** del menú → carga `GameScene.unity`.

### Controles

| Acción | Tecla |
|---|---|
| Mover | `WASD` |
| Apuntar | Mouse |
| Disparar | Click izquierdo |
| Recargar | `R` |
| Zoom cámara | Rueda del mouse |

---

## Estructura del proyecto

```
Assets/
├── _Project/              # código y assets propios del TP
│   ├── Animations/        # AnimatorControllers
│   ├── Scenes/            # MainMenu, GameScene
│   └── Scripts/
│       ├── Camera/        # CameraFollow
│       ├── Controllers/   # PlayerController
│       └── ...
├── ThirpartyAssets/       # Mixamo, packs externos (en LFS)
└── InputSystem_Actions.inputactions
Docs/
└── consigna.md            # enunciado del TP
```

---

## Notas sobre Git LFS

- Patrones bajo LFS: `*.fbx`, `*.blend`, `*.psd`, `*.wav`, `*.mp3`, `*.ogg`, `*.mp4` (ver `.gitattributes`).
- GitHub free permite 1 GB de bandwidth LFS por mes — suficiente para el ciclo de evaluación de este TP.
- Si al hacer pull aparecen FBX rotos, correr `git lfs pull` para re-materializar.
