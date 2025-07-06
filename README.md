# Outer Wilds Mod Jam 5 Example

Example setup to make an entry for [Outer Wilds Mod Jam 5](https://github.com/xen-42/ow-mod-jam-5-base).

The most important takeaways are:

#1 Have a center body with `"centerOfSolarSystem": true` set. All other bodies in your mod must be positioned relative to this. Doesn't have to be a star, can even just be an empty point in space, just has to be there!

#2 Have a platform with 
```cs
  "extras": {
    "isPlatform": true
  },
```
which will be the starting point for your mod within the central station Nomai vessel!
