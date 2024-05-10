# Xana-Optimized

### Rules to follow, during your development for XANA APP

> [!Important]
> Coding rules

|  Name  | Rule |
| --- | --- |
| `Classes, Methods, Enumerations, Public, fields, public, properties, Namespaces` |  **PascalCase** |
| `Private variables, Local variables, Parameters` | **camelCase** |

> [!TIP]
> When Codding

- **Use meaningful names**, Variable names must be descriptive, clear, and unambiguous because they represent a thing or state. So use a noun when naming them except when the variable is of the type bool.
- You can prefix private member variables with an underscore (_) to differentiate them from local variables.
- Naming convention is unaffected by modifiers such as const, static, readonly, etc.
- Naming should be like MyRpc instead of MyRPC.
- Use **one variable declaration per line** as it enhances readability.
- Names of interfaces start with I, e.g. IInterface.
- Use pascal case without special symbols or underscores for Namespace and to be declarations at the top.
- Use pascal case nouns for class names.
- **Class member ordering**:
  - Within each group, elements should be in the following order:
    - Public.
    - Internal.
    - Protected internal.
    - Protected.
    - Private.
   - Group class members into following sequence
     - Fields 
     - Properties 
     - Events / Delegates 
     - Constructors
     - Monobehaviour Methods (Awake, Start, OnEnable, OnDisable, OnDestroy, etc.) 
     - Public Methods 
     - Private Methods
     - Where possible, group interface implementations together.
- Use pascal case for method name and should start with Verb. 
- Group dependent and/or similar methods together.
- Use pascal case for enum names and values.
- **Name the event with a verb phrase**: Choose a name that communicates the state change accurately. Use the present or past participle to indicate events “before” or “after.” For example, specify “OpeningDoor” for an event before opening a door or “DoorOpened” for an event afterward. “GameEvents_OpeningDoor” or “GameEvents_DoorOpened.”
Don’t omit braces, even for single-line statements and also don’t remove braces from nested multi-line statements having single-line statements.
- Add horizontal spaces to decrease code density.
  **Example:** for (int i = 0; i < 100; i++) { DoSomething(i); }
- Use the single-responsibility principle when designing classes and methods.
- Don’t set up your method to work in two different modes based on a flag. Make two methods with distinct names. As it can lead to tangled implementation or broken single-responsibility.
- Avoid duplicate or repetitious logic.
- **Make your code self-explationary**. If you need to add a comment to explain a convoluted tangle of logic, restructure your code to be more obvious. Then you won’t need the comment.
- Place the comment on a separate line when possible, not at the end of a line of code. 
- Keep the comment near the code.
- **Don’t place name in comments**, Example: // added by devA or devB
- **Use a tooltip** instead of a comment for serialized fields.
- Remove commented out code.

# Other Document Links
- **XANA Project Standard**
[Link](https://docs.google.com/document/d/1Y-YLolfLzlI1WTw8sfxHS0GFKwmVQwcmWBnd0FixknU/edit?usp=sharing)
- **Create IOS Build**
[Link](https://docs.google.com/document/d/1M7ZI8oH9-gjBJ-zj8iKv-OftSxX3CUWjbjgQhy7j-7Y/edit?usp=sharing)

