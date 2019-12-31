# Car driving AI
 Unity 3d project utilizing neural networks to teach a car how to drive using neural network and genetic algorithm (VERY simple version). 
 
Project description:
 The car drives forward automaticaly, the only input of the car is its rotation (-1 to 1). The car has five distance sensors calculating  distance from the walls (colliders) that are on the sides of the road.
 
Car game object:
 Car consists of a Transform, Rigidbody, Car model (from model form asset store), Box collider, NeuralNet script, AIdriver script
 
 The car has a neural network that consists of 5 input nodes (float value from each car sensor), one hidden layer with 3 nodes, output layer with 1 node (for the car rotation). Neural network has for different bias for each of the nidden layer nodes and the output node. 
 
 To start You need to reference the NeuralNetwork script, for example:
 
 Brain = GetComponent<NeuralNet>();
 
 Then You have to initialize the neural network (set the number of input the number of hidden layers and output layers):
 
 Brain.InitializeNeuralNetwork(5, hiddenLayers, 1, 2); // where InitializeNeuralNetwork(nr of input nodes, hidden layer list, nr of outputs, activation function type)
 
 where:
 - hidden layer list: // a list that defines the hidden nodes setup exp. {2,3,3} three layers first layer 2 nodes second layer 3 nodes third layer 3 nodes
 - activation function type: 0- Sigmoid, 1- RELU, 2- Tanh
 
 You can randomize weights and bias arrays because in default they are 0.
 
 Brain.SetValuesToArray(Brain.weightsArray, Random.Range(-1f, 1f));
 Brain.SetValuesToArray(Brain.biasArray, Random.Range(-1f, 1f));

To calculate the output of the neural network use:

Brain.CalcuateNeuralNetwork();

Genetic algorithm:
Each car has a score that consists of the sum of the distance calculated from each of the car sensors for each frame. The cars ar then put into an array sorted from the highest to the lowest score. 
The algorithm mutates the weights and bias nodes of the low scor cars so they resemble the best score cars.

You can see it in action at: https://www.youtube.com/watch?v=GppaZ00be2s
