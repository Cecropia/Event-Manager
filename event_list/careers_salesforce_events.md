# Careers-Salesforce List of Events to be used on Event Manager

---

#### EVENT `careers_salesforce_after_skill_created` 

#### EVENT ~~`careers_salesforce_after_skill_updated`~~

#### EVENT ~~`careers_salesforce_after_skill_deleted`~~

---

#### EVENT `careers_salesforce_after_qualification_created` 

#### EVENT ~~`careers_salesforce_after_qualification_updated`~~

#### EVENT ~~`careers_salesforce_after_qualification_deleted`~~

---

#### EVENT `careers_salesforce_after_profile_created` 

#### EVENT ~~`careers_salesforce_after_profile_updated`~~

#### EVENT ~~`careers_salesforce_after_profile_deleted`~~

---

#### EVENT `careers_salesforce_after_joboffer_created` 

#### EVENT ~~`careers_salesforce_after_joboffer_updated`~~

#### EVENT ~~`careers_salesforce_after_joboffer_deleted`~~

---

#### EVENT `careers_salesforce_after_[BEFORE||AFTER]_[ENTITY]_[EVENT]` 

###### DESCRIPTION EXAMPLE

Event description example

###### PAYLOAD EXAMPLE

``` json
{
  "Payload":[
      // expected list of elements
      // same format as the entity ENDPOINT
  ],
  "timestamp": "2020-05-22T21:28:06.496Z",
  "extraParams": {"name": "value"}, // extra parameters if needed
}

```

---
---

#### EVENT `careers_salesforce_after_skill_created` 

###### DESCRIPTION

###### PAYLOAD

``` json
{
  "Payload":[
    {
      "Type" : "Hard Skill",
      "Name" : "React Framework",
      "Id" : "97a08d27-b31c-4783-a2b7-87dc2666a18d"
    }
  ],
  "timestamp": "2020-05-22T21:28:06.496Z",
  "extraParams": {"name": "value"},
}

```

#### EVENT ~~`careers_salesforce_after_skill_updated`~~

#### EVENT ~~`careers_salesforce_after_skill_deleted`~~

---

#### EVENT `careers_salesforce_after_qualification_created` 

###### DESCRIPTION

###### PAYLOAD

``` json
{
    "Payload": [
      {
          "Name":"Technical Degree",
          "Minimum_Required":"Degree",
          "Id":"1574d156-14b6-4d14-a01d-4931274aeb48",
          "Digital_Support":true,
          "Description":"Technical Degree"
      },
      {
          "Name":"Javascript Course",
          "Minimum_Required":"Course",
          "Id":"aaa066aa-a56f-40b7-a698-d780f9bbac38",
          "Digital_Support":true,
          "Description":"Technical Course"
      }
    ],
    "timestamp": "2020-05-22T21:28:06.496Z",
    "extraParams": {"name": "value"},
}

```

#### EVENT ~~`careers_salesforce_after_qualification_updated`~~

#### EVENT ~~`careers_salesforce_after_qualification_deleted`~~

---

#### EVENT `careers_salesforce_after_profile_created` 

###### DESCRIPTION

###### PAYLOAD

``` json
{
   "Payload":[
      {
         "skills":[
            {
               "SkillType":"Hard Skill",
               "SkillName":"Angular Framework",
               "relation":"Rating",
               "ProfileName":"Angular",
               "Id":"aa7c0e65-1211-4e55-881a-50ac353441b4",
               "Amount":3
            }
         ],
         "qualifications":[
            {
               "Name":"Computer Degree",
               "MinimumRequired":"Degree",
               "Id":"7ef27a31-bb1d-409e-905f-a8b17886ea09",
               "DigitalSupportRequired":true,
               "Description":"Computer Degree"
            }
         ],
         "Name":"Base Profile",
         "Id":"6b32e6f7-ba83-418f-b35d-4fda11e1e254"
      },
      {
         "skills":[
            {
               "SkillType":"Hard Skill",
               "SkillName":"React Framework",
               "relation":"Rating",
               "ProfileName":"ReactJS",
               "Id":"1f7cb443-a8f0-4b46-b9ef-049e8bbdeb4c",
               "Amount":3
            }
         ],
         "qualifications":[
            {
               "Name":"Technical Degree",
               "MinimumRequired":"Degree",
               "Id":"66f1d666-037c-4414-a2e6-c646d55d0295",
               "DigitalSupportRequired":true
            }
         ],
         "Name":"Technical Profile",
         "Id":"00073d0a-e3a4-45a0-885b-13b11a2767e5"
      }
   ],
  "timestamp": "2020-05-22T21:28:06.496Z",
  "extraParams": {"name": "value"},
}
```

#### EVENT ~~`careers_salesforce_after_profile_updated`~~

#### EVENT ~~`careers_salesforce_after_profile_deleted`~~

---

#### EVENT `careers_salesforce_after_joboffer_created` 

###### DESCRIPTION

###### PAYLOAD

``` json
{

  "Payload": [
    {
        "profiles":[
          {
              "skills":[
                {
                    "SkillType":"Soft Skill",
                    "SkillName":"Time Manager",
                    "relation":"Rating",
                    "ProfileName":"Time Manager",
                    "Id":"496138dd-58c6-430d-a2f5-02ab846c9dc6",
                    "Amount":5
                },
                {
                    "SkillType":"Hard Skill",
                    "SkillName":"Android",
                    "relation":"Years of Experience",
                    "ProfileName":"Android",
                    "Id":"78cb0a4f-a719-42d1-ae4c-10357a31e41e",
                    "Amount":4
                },
                {
                    "SkillType":"Soft Skill",
                    "SkillName":"Work Independently and as part of a group",
                    "relation":"Rating",
                    "ProfileName":"Work Independently and as part of a group",
                    "Id":"adfbd355-86e3-4668-891c-0aec996837bd",
                    "Amount":5
                }
              ],
              "qualifications":[
                {
                    "Name":"Android Tools",
                    "MinimumRequired":"Course",
                    "Id":"455937fb-c9a3-48ec-8770-1ea6d250fb72",
                    "DigitalSupportRequired":true
                }
              ],
              "Name":"Android Developer",
              "Id":"cd7a4c59-7552-4e74-90b5-f44dd2a663c0",
              "Dependency":"Desired"
          }
        ],
        "Name":"Mobile Network Engineer",
        "ExpireOn":"2020-11-30",
        "Description":"Basic Requirements:\r\n- 6+ years of development experience.\r\n- 4+ years of Android development experience.\r\n- Fluency in Android development tools.\r\n- Strong knowledge in network architecture and specifically Android.\r\n- Familiarity with networking and communications protocols.",
        "ContractType":"Full Time",
        "attachements":[
          {
              "Title":"Mobile Network Engineer Offer",
              "FileExtension":"jpg"
          }
        ],
        "AssignedLocation":"Colombia",
        "Account":{
          "Name":"Cecropia Solutions",
          "Id":"0010U000012GDMVQA4"
        }
    }
  ],
  "timestamp": "2020-05-22T21:28:06.496Z",
  "extraParams": {"name": "value"},
}

```

#### EVENT ~~`careers_salesforce_after_joboffer_updated`~~

#### EVENT ~~`careers_salesforce_after_joboffer_deleted`~~

---
