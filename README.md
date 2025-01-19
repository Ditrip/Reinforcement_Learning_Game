# Unity game with ML elements
This Unity-based game incorporates machine learning elements from the Unity [ML-Agents]((https://github.com/Unity-Technologies/ml-agents)) library. 
The objective is to guide the player to the final destination while 
navigating around pre-defined random obstacles that appear along the way

## Dependencies
- Unity (editor version): 2021.3.11f1
- Python: 3.10.12
  - ml-agents: 1.1.0

## Installation
- Unity: The Unity editor can be installed from [Unity-hub](https://unity.com/download) ([requirements](https://docs.unity3d.com/hub/manual/InstallHub.html)).
- Python ([Poetry](https://python-poetry.org/docs/)): Poetry installs all project dependencies in virtual env.

Linux:
```
# Optional: To install the virtual env in project 
# set the next env var 
# export POETRY_VIRTUALENVS_IN_PROJECT=true

poetry install --no-root

# check if ml-agent is installed successfully
poetry run mlagents-learn --help 
```

## Model Training
Ml-agent requires a `yaml` configuration file that 
contains the model's [hyperparameters](https://unity-technologies.github.io/ml-agents/Training-Configuration-File/). 

Linux:
```
# syntax: 
# mlagents-learn [options] /path/to/config.yaml 
poetry run mlagents-learn --run-id "test" Assets/AIModels/ppo/basic.yaml
```

### Applying the model into the game
To apply the trained model, use the `Agent.onnx` file in the `results` folder 
created after training and apply it to the agent's `Behavior Parameters` script in the Unity editor.
- Location of `Behavior Parameters`: Assets/Prefabs/Level/Agent/Behavior Parameters


