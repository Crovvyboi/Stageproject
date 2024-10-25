import csv
from datetime import datetime
import os
from pymongo import MongoClient
from pymongo.server_api import ServerApi

def send_csv_to_mongo(dir_output, filename):
    json = read_from_dir(dir_output, filename)
    json = add_metadata(json)

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


def add_metadata(totalJson):
    print(totalJson)
    # For each overzicht, count rows, added/removed columns and changes. Add as metadata (including time and date)
    added_column_count = 0
    removed_column_count = 0
    changes_count = 0


    data = totalJson['data']
    for entry in data:
        # Count added columns
        added_columns = entry['added_columns']
        if added_columns != 'n/a':
            split_added = str(added_columns).split(',')
            if len(split_added) > 0:
                added_column_count += len(split_added)
    	
        # Count removed columns
        removed_columns = entry['removed_columns']
        if removed_columns != 'n/a':
            split_removed = str(removed_columns).split(',')
            if len(split_removed) > 0:
                removed_column_count += len(split_removed)
    
        # Count differences
        found_diffs = entry['found_diffs']
        for diff in found_diffs:
            changes_count += 1


    json = {
        'taak': {
            'uitvoer_date':  datetime.today(),
            'added_column_count': added_column_count,
            'removed_column_count': removed_column_count,
            'changes_count': changes_count,
            'diff_data': totalJson
        } 
    }

    return json


def read_from_dir(dir_output, filename):
    totalJson = {'data': []}

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
                    totalJson['data'].append(tempJson)
                    tempJson == {}
                founddiffs = 0
                tempJson = {
                    'overzicht': list[1],
                    'removed_columns': '',
                    'added_columns': '',
                    'found_diffs': []
                }

            elif list[0] == 'Removed columns:':
                if tempJson != {}:
                    tempJson['removed_columns'] = list[1]
            
            elif list[0] == 'Added columns:':
                if tempJson != {}:
                    tempJson['added_columns'] = list[1]
                
            else:
                if tempJson != {}:
                    diffObject = {
                            'OnEntryID': list[0],
                            'OnColumn': list[1],
                            'ValueA': list[2],
                            'ValueB': list[3]
                        }
                    
                    tempJson['found_diffs'].append(diffObject)

                    founddiffs += 1



        if tempJson != {}:
            totalJson['data'].append(tempJson)
                


    return totalJson