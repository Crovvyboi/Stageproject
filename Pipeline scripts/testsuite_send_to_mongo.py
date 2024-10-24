import csv
import os
from pymongo import MongoClient
from pymongo.server_api import ServerApi

def send_csv_to_mongo(dir_output, filename):
    json = read_from_dir(dir_output, filename)

    # Replace the placeholder with your Atlas connection string
    uri = "mongodb+srv://riksmolders:Mieper99-Mieper99@cluster0.z5tuv.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0"

    # Create a MongoClient with a MongoClientOptions object to set the Stable API version
    client = MongoClient(uri)
    try:
        # Connect the client to the server (optional starting in v4.7)
        client._connect()
        print("Connected!")

        database = client["analyse_data"]
        collection = database["taken"]
        x = collection.insert_one(json)
        print(x)

    except:
        print("Something went wrong, but do not care")
        client.close()
    finally:
        # Ensures that the client will close when you finish/error    
        client.close()
        print("Closed!")


def read_from_dir(dir_output, filename):
    totalJson = {'result': []}

    # Read file from dir
    if os.path.isfile(dir_output + filename):
        datastring = []
        with open(dir_output + filename, 'r', newline='') as csvfile:
            reader = csv.reader(csvfile, skipinitialspace=True, quotechar='"')
            for row in reader:
                if row and row != ['']:
                    datastring.append(row)

        print(datastring)

        tempJson = {}
        founddiffs = 0
        for list in datastring:
            if list[0] == 'File: ':
                if tempJson != {}:
                    totalJson['result'].append(tempJson)
                    tempJson == {}
                founddiffs = 0
                tempJson = {
                    'overzicht': list[1],
                    'removed columns': '',
                    'added columns': '',
                    'found diffs': []
                }

            elif list[0] == 'Removed columns:':
                if tempJson != {}:
                    tempJson['removed columns'] = list[1]
            
            elif list[0] == 'Added columns:':
                if tempJson != {}:
                    tempJson['added columns'] = list[1]
                
            else:
                if tempJson != {}:
                    diffObject = {
                            'OnEntryID': list[0],
                            'OnColumn': list[1],
                            'ValueA': list[2],
                            'ValueB': list[3]
                        }
                    
                    tempJson['found diffs'].append(diffObject)

                    founddiffs += 1



        if tempJson != {}:
            totalJson['result'].append(tempJson)
                


    return totalJson