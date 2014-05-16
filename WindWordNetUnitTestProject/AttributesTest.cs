using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Erwine.Leonard.T.WordServer.Common;

namespace WindWordNetUnitTestProject
{
    [TestClass]
    public class AttributesTest
    {
        /*
Table of Contents
 

NAME 
List of WordNet lexicographer file names and numbers 
DESCRIPTION 
During WordNet development synsets are organized into forty-five lexicographer files based on syntactic category and logical groupings. grind(1WN) processes these files and produces a database suitable for use with the WordNet library, interface code, and other applications. The format of the lexicographer files is described in wninput(5WN) . 
A file number corresponds to each lexicographer file. File numbers are encoded in several parts of the WordNet system as an efficient way to indicate a lexicographer file name. The file lexnames  lists the mapping between file names and numbers, and can be used by programs or end users to correlate the two. 

File Format 
Each line in lexnames  contains 3 tab separated fields, and is terminated with a newline character. The first field is the two digit decimal integer file number. (The first file in the list is numbered 00 .) The second field is the name of the lexicographer file that is represented by that number, and the third field is an integer that indicates the syntactic category of the synsets contained in the file. This is simply a shortcut for programs and scripts, since the syntactic category is also part of the lexicographer file's name.
         */
        [TestMethod]
        public void SyntacticCategoryTestMethod()
        {
            /*
Syntactic Category 
The syntactic category field is encoded as follows: 

1     NOUN
             */
            short expected = 1;
            PartOfSpeech target = PartOfSpeech.Noun;
            short actual = (short)target;
            Assert.AreEqual(expected, actual);

            /*
2     VERB 
             */
            expected = 2;
            target = PartOfSpeech.Verb;
            actual = (short)target;
            Assert.AreEqual(expected, actual);

            /*
3     ADJECTIVE 
             */
            expected = 3;
            target = PartOfSpeech.Adjective;
            actual = (short)target;
            Assert.AreEqual(expected, actual);

            /*
4     ADVERB 
             */
            expected = 4;
            target = PartOfSpeech.Adverb;
            actual = (short)target;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void LexicographerFilesTestMethod()
        {
            /*
Lexicographer Files 
The names of the lexicographer files and their corresponding file numbers are listed below along with a brief description each file's contents. 

File Number   Name   Contents   

00  adj.all  all adjective clusters 
             */
            short expectedValue = 0;
            LexicographerFile target = LexicographerFile.Adj_All;
            short actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            PartOfSpeech expectedPos = PartOfSpeech.Adjective;
            PartOfSpeech actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            string expectedDescription = "All adjective clusters";
            string actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
01  adj.pert  relational adjectives (pertainyms) 
             */
            expectedValue = 1;
            target = LexicographerFile.Adj_Pert;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Adjective;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
02  adv.all  all adverbs  
             */
            expectedValue = 2;
            target = LexicographerFile.Adv_All;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Adverb;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "All adverbs";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
03  noun.Tops  unique beginner for nouns  
             */
            expectedValue = 3;
            target = LexicographerFile.Noun_Tops;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Noun;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Unique beginner for nouns";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            // TODO: Fix targets and descriptions

            /*
04  noun.act  nouns denoting acts or actions  
             */
            expectedValue = 4;
            target = LexicographerFile.Noun_Act;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Noun;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Nouns denoting acts or actions";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
05  noun.animal  nouns denoting animals  
             */
            expectedValue = 5;
            target = LexicographerFile.Noun_Animal;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Noun;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
06  noun.artifact  nouns denoting man-made objects  
             */
            expectedValue = 6;
            target = LexicographerFile.Noun_Artifact;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Noun;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
07  noun.attribute  nouns denoting attributes of people and objects  
             */
            expectedValue = 7;
            target = LexicographerFile.Noun_Attribute;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Noun;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
08  noun.body  nouns denoting body parts  
             */
            expectedValue = 8;
            target = LexicographerFile.Noun_Body;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Noun;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
09  noun.cognition  nouns denoting cognitive processes and contents  
             */
            expectedValue = 9;
            target = LexicographerFile.Noun_Cognition;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Noun;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
10  noun.communication  nouns denoting communicative processes and contents  
             */
            expectedValue = 10;
            target = LexicographerFile.Noun_Communication;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Noun;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
11  noun.event  nouns denoting natural events  
             */
            expectedValue = 11;
            target = LexicographerFile.Noun_Event;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Noun;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
12  noun.feeling  nouns denoting feelings and emotions  
             */
            expectedValue = 12;
            target = LexicographerFile.Noun_Feeling;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Noun;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
13  noun.food  nouns denoting foods and drinks  
             */
            expectedValue = 13;
            target = LexicographerFile.Noun_Food;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Noun;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
14  noun.group  nouns denoting groupings of people or objects  
             */
            expectedValue = 14;
            target = LexicographerFile.Noun_Group;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Noun;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
15  noun.location  nouns denoting spatial position  
             */
            expectedValue = 15;
            target = LexicographerFile.Noun_Location;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Noun;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
16  noun.motive  nouns denoting goals  
             */
            expectedValue = 16;
            target = LexicographerFile.Noun_Motive;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Noun;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
17  noun.object  nouns denoting natural objects (not man-made)  
             */
            expectedValue = 17;
            target = LexicographerFile.Noun_Object;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Noun;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
18  noun.person  nouns denoting people  
             */
            expectedValue = 18;
            target = LexicographerFile.Noun_Person;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Noun;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
19  noun.phenomenon  nouns denoting natural phenomena  
             */
            expectedValue = 19;
            target = LexicographerFile.Noun_Phenomenon;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Noun;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
20  noun.plant  nouns denoting plants  
             */
            expectedValue = 20;
            target = LexicographerFile.Noun_Plant;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Noun;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
21  noun.possession  nouns denoting possession and transfer of possession 
             */
            expectedValue = 21;
            target = LexicographerFile.Noun_Possession;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Noun;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /* 
22  noun.process  nouns denoting natural processes  
             */
            expectedValue = 22;
            target = LexicographerFile.Noun_Process;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Noun;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
23  noun.quantity  nouns denoting quantities and units of measure  
             */
            expectedValue = 23;
            target = LexicographerFile.Noun_Quantity;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Noun;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
24  noun.relation  nouns denoting relations between people or things or ideas 
             */
            expectedValue = 24;
            target = LexicographerFile.Noun_Relation;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Noun;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /* 
25  noun.shape  nouns denoting two and three dimensional shapes  
             */
            expectedValue = 25;
            target = LexicographerFile.Noun_Shape;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Noun;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
26  noun.state  nouns denoting stable states of affairs  
             */
            expectedValue = 26;
            target = LexicographerFile.Noun_State;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Noun;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
27  noun.substance  nouns denoting substances  
             */
            expectedValue = 27;
            target = LexicographerFile.Noun_Substance;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Noun;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
28  noun.time  nouns denoting time and temporal relations  
             */
            expectedValue = 28;
            target = LexicographerFile.Noun_Time;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Noun;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
29  verb.body  verbs of grooming, dressing and bodily care  
             */
            expectedValue = 29;
            target = LexicographerFile.Verb_Body;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Verb;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
30  verb.change  verbs of size, temperature change, intensifying, etc.  
             */
            expectedValue = 30;
            target = LexicographerFile.Verb_Change;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Verb;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
31  verb.cognition  verbs of thinking, judging, analyzing, doubting  
             */
            expectedValue = 31;
            target = LexicographerFile.Verb_Cognition;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Verb;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
32  verb.communication  verbs of telling, asking, ordering, singing  
             */
            expectedValue = 32;
            target = LexicographerFile.Verb_Body;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Verb;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
33  verb.competition  verbs of fighting, athletic activities  
             */
            expectedValue = 33;
            target = LexicographerFile.Verb_Body;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Verb;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
34  verb.consumption  verbs of eating and drinking  
             */
            expectedValue = 34;
            target = LexicographerFile.Verb_Body;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Verb;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
35  verb.contact  verbs of touching, hitting, tying, digging  
             */
            expectedValue = 35;
            target = LexicographerFile.Verb_Body;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Verb;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
36  verb.creation  verbs of sewing, baking, painting, performing  
             */
            expectedValue = 36;
            target = LexicographerFile.Verb_Body;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Verb;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
37  verb.emotion  verbs of feeling  
             */
            expectedValue = 37;
            target = LexicographerFile.Verb_Body;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Verb;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
38  verb.motion  verbs of walking, flying, swimming  
             */
            expectedValue = 38;
            target = LexicographerFile.Verb_Body;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Verb;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
39  verb.perception  verbs of seeing, hearing, feeling  
             */
            expectedValue = 39;
            target = LexicographerFile.Verb_Body;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Verb;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
40  verb.possession  verbs of buying, selling, owning  
             */
            expectedValue = 40;
            target = LexicographerFile.Verb_Body;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Verb;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
41  verb.social  verbs of political and social activities and events  
             */
            expectedValue = 41;
            target = LexicographerFile.Verb_Body;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Verb;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
42  verb.stative  verbs of being, having, spatial relations  
             */
            expectedValue = 42;
            target = LexicographerFile.Verb_Body;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Verb;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
43  verb.weather  verbs of raining, snowing, thawing, thundering  
             */
            expectedValue = 43;
            target = LexicographerFile.Verb_Body;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Verb;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);

            /*
44  adj.ppl  participial adjectives 
             */
            expectedValue = 44;
            target = LexicographerFile.Adj_Pert;
            actualValue = (short)target;
            Assert.AreEqual(expectedValue, actualValue);
            expectedPos = PartOfSpeech.Adjective;
            actualPos = PosValueAttribute.GetPartOfSpeech<LexicographerFile>(target);
            Assert.AreEqual(expectedPos, actualPos);
            expectedDescription = "Relational adjectives (pertainyms)";
            actualDescription = PosValueAttribute.GetDescription<LexicographerFile>(target);
            Assert.AreEqual(expectedDescription, actualDescription);
        }
        /*   

NOTES 
The lexicographer files are not included in the WordNet database package. 
ENVIRONMENT VARIABLES (UNIX) 
WNHOME Base directory for WordNet. Default is /usr/local/WordNet-3.0 . WNSEARCHDIR Directory in which the WordNet database has been installed. Default is WNHOME/dict . 
REGISTRY (WINDOWS) 
HKEY_LOCAL_MACHINE\SOFTWARE\WordNet\3.0\WNHome Base directory for WordNet. Default is C:\Program Files\WordNet\3.0 . 
FILES 
lexnames list of lexicographer file names and numbers 
SEE ALSO 
grind(1WN) , wnintro(5WN) , wndb(5WN) , wninput(5WN) . 



Table of Contents


•NAME
•DESCRIPTION
◦File Format
◦Syntactic Category
◦Lexicographer Files
•NOTES
•ENVIRONMENT VARIABLES (UNIX)
•REGISTRY (WINDOWS)
•FILES
•SEE ALSO

         */

    }
}
