using UnityEngine;
using System.Collections;

public class baseCharacter : MonoBehaviour {

    private string characterClassName;
    private string characterClassDescription;
    //stats for character
    private double stamina;
    private double endurance;
    private double strength;
    private double intellect;
    private double agility;
    private double armor;


    //enable us to set and get character class name
    public string CharacterClassName
    {
        get { return characterClassName; }
        set { characterClassName = value; }
    }
    //enable us to set and get character class description
    public string CharacterClassDescription
    {
        get { return characterClassDescription; }
        set { characterClassDescription = value; }
    }
    //enable us to set and get character class stamina
    public double Stamina
    {
        get { return stamina; }
        set { stamina = value; }
    }
    //enable us to set and get character class endurance
    public double Endurance
    {
        get { return endurance; }
        set { endurance = value; }
    }
    //enable us to set and get character class strength
    public double Strength
    {
        get { return strength; }
        set { strength = value; }
    }
    //enable us to set and get character class intellect
    public double Intellect
    {
        get { return intellect; }
        set { intellect = value; }
    }
    //enable us to set and get character class agility
    public double Agility
    {
        get { return agility; }
        set { agility = value; }
    }
    //enable us to set and get character class Armor
    public double Armor
    {
        get { return armor; }
        set { armor = value; }
    }
}
