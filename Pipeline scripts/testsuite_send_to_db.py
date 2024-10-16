import csv
import json
import os

import psycopg2


def send_csv_to_db(dir_output, filename):
    conn = psycopg2.connect(
        database = 'postgres',
        user = 'postgres.gjamzttckelxaokprjjk',
        password = 'Mieper99-Mieper99',
        host = 'aws-0-eu-central-1.pooler.supabase.com',
        port = 6543
    )

    # Get data from results file
    if os.path.isfile(dir_output + filename):
        datastring = []
        with open(dir_output + filename, 'r', newline='') as csvfile:
            reader = csv.reader(csvfile, skipinitialspace=True, quotechar='"')
            for row in reader:
                if row and row != ['']:
                    datastring.append(row)

        print(datastring)

        cursor = conn.cursor()

        # Format data
        # - kept parameters
        selected_file = ''
        selected_file_id = -1
        taak_id = -1

        for entry in datastring:
            if entry[0] != 'OnEntryID':
                if entry[0] == 'File: ':
                    # File name, check if overzicht is in overzichten
                    cursor.execute('SELECT overzicht_id, overzicht_naam FROM overzichten')
                    rows = cursor.fetchall()

                    parsedrows = []
                    parsedrowsIDs = []
                    for row in rows:
                        parsedrowsIDs.append(row[0])
                        parsedrows.append(row[1])

                    if not entry[1] in parsedrows:
                        # overzicht does not exist in db, insert
                        cursor.execute('INSERT INTO overzichten(overzicht_naam) VALUES(\'' + entry[1] + '\')')
                        conn.commit()
                        print('Inserted ' + entry[1] + ' into overzichten!')
                        selected_file = entry[1]

                        # Get new id
                        cursor.execute('SELECT overzicht_id FROM overzichten WHERE overzicht_naam = \'' + selected_file + '\'')
                        selected_file_id = cursor.fetchone()[0]
                    else:
                        selected_file = entry[1]
                        row_index = parsedrows.index(entry[1])
                        selected_file_id = parsedrowsIDs[row_index]

                    # Insert taak into db
                    cursor.execute("INSERT INTO taken(overzicht_id) VALUES(" + str(selected_file_id) + ")")
                    conn.commit()
                    print('Inserted taak on overzicht id ' + str(selected_file_id))

                    # Get new taak id
                    cursor.execute('SELECT taak_id FROM taken ORDER BY taak_id DESC')
                    taak_id = cursor.fetchone()[0]

                elif entry[0] == 'Removed columns:':
                    # Removed column, add to gevonden_verschillen with diff_type = 'column' and value_a as main value
                    if entry[1] != "n/a":
                        cursor.execute("INSERT INTO gevonden_verschillen(diff_type, value_a, taak_id) VALUES('Removed column', '" + entry[1] + "', " + str(taak_id) + ")")
                        conn.commit()



                elif entry[0] == 'Added columns:':
                    # Added column, add to gevonden_verschillen with diff_type = 'column' and value_b as main value
                    if entry[1] != "n/a":
                        cursor.execute("INSERT INTO gevonden_verschillen(diff_type, value_b, taak_id) VALUES('Added column', '" + entry[1] + "', " + str(taak_id) + ")")
                        conn.commit()

                else:
                    # Regular entry, insert
                    value_a = entry[2]
                    value_a = value_a.replace("'", "&#39;")
                    value_b = entry[3]
                    value_b = value_b.replace("'", "&#39;")
                    cursor.execute("INSERT INTO gevonden_verschillen(on_entry_id, diff_type, on_column, value_a, value_b, taak_id) " +
                                   "VALUES(" + str(entry[0]) + ", 'Entry difference', '" + entry[1] + "', '" + value_a + "', '" + value_b + "', '" + str(taak_id) + "')")
                    conn.commit()

        add_taak_details(conn, dir_output, filename)
        add_taak_metadata()

        cursor.close()
        conn.close()

        #  Temp -------------------------------------------------------------
        remove_resfile(dir_output, filename)
        # -------------------------------------------------------------------

def remove_resfile(dir_output, filename):
    if os.path.exists(dir_output + filename) and os.path.isfile(dir_output + filename):
        os.remove(dir_output + filename)
        print("file deleted")
    else:
        print("file not found")

def add_taak_metadata():
    print('Writing metadata')

def add_taak_details(conn, dir_output, filename):
    print('Writing taak details')

    if os.path.isfile(dir_output + filename):
        datastring = []
        with open(dir_output + filename, 'r', newline='') as csvfile:
            reader = csv.reader(csvfile, skipinitialspace=True, quotechar='"')
            for row in reader:
                if row:
                    datastring.append(row)

        cursor = conn.cursor()

        taak_id = -1
        added = 0
        removed = 0
        changed = 0
        for entry in datastring:
            if entry[0] == 'File: ':
                # Get taak_id from db
                cursor.execute("Select * From taken Join overzichten on taken.overzicht_id = overzichten.overzicht_id Where overzicht_naam = '" + entry[1] + "' Order By taak_id Desc Limit 1")
                taak_id = cursor.fetchone()[0]

                added = 0
                removed = 0
                changed = 0
            elif entry[0] == '':
                # Insert into db
                cursor.execute("INSERT INTO taak_details(taak_id, found_diffs, found_added_columns, found_removed_columns) VALUES(" + str(taak_id) + ", " + str(changed) + ", " + str(added) + ", " + str(removed) + ")")
                conn.commit()
            elif entry[0] != 'OnEntryID':
                if entry[0] == 'Added columns:':
                    if entry[1] != 'n/a':
                        added_entry = entry[1].split()
                        added += len(added_entry)
                elif entry[0] == 'Removed columns:':
                    if entry[1] != 'n/a':
                        removed_entry= entry[1].split()
                        removed += len(removed_entry)
                else:
                    changed += 1



