import csv
import os
import difflib
import argparse
import xml.dom.minidom
import filecmp

from typing import Set

from testsuite_send_to_db import send_csv_to_db

parser = argparse.ArgumentParser(description='Create diff files for AB tests')
# parser.add_argument('--o', type=str, required=True, help='Output directory for found differences')
# parser.add_argument('--a', type=str, required=True, help='Directory containing files to diff from environment A')
# parser.add_argument('--b', type=str, required=True, help='Directory containing files to diff from environment B')

def get_file_content(file_name):
    with open(file_name, 'r', encoding='cp1252') as file:
        if file_name.endswith('.xml'):
            dom = xml.dom.minidom.parseString(file.read())
            return dom.toprettyxml()

        return file.read()


def create_export_file(file_name, content):
    with open(file_name, 'w', encoding='utf-8') as output_file:
        output_file.write(content)

    print(f'Created export: {file_name}')

def find_files(target_dir) -> Set[str]:
    """
    Find the set of files in the target directory with the allowed extension.
    """
    allowed_extensions = ['.csv', '.xml']
    return set([f for f in os.listdir(target_dir) if os.path.splitext(f)[1] in allowed_extensions])

# ----------------------------------
def convert_to_detail_string(file_lines_a, file_lines_b, det_file_content, dir_output):
    """"
        Convert diff string into a csv file
            Header is the name of the file its based off

            CSV file is configured as
            Header - From which file?
            On entry id, On variable, Value A, Value B

            If an entry was added, Value A is 'n/a'
            If an entry was removed, Value B is 'n/a'
    """
    # Get headers
    header_a = file_lines_a[0]
    header_b = file_lines_b[0]

    removed_heads = []
    added_heads = []
    # Determine if there's a difference in headers
    if header_a != header_b:
        split_head_a = header_a.split(',')
        split_head_b = header_b.split(',')



        # Find columns in a not in b
        for header in split_head_a:
            if header not in split_head_b:
                removed_heads.append(header)
                # Remove all values at index
                index = split_head_a.index(header)
                lines_a = []
                for entry in file_lines_a:
                    splitline = entry.split(',')
                    splitline.pop(index)
                    entry = ','.join(splitline)
                    print(entry)
                    lines_a += [entry]
                file_lines_a = lines_a

        # Find columns in b not in a
        for header in split_head_b:
            if header not in split_head_a:
                added_heads.append(header)
                # Remove all values at index
                index = split_head_b.index(header)
                lines_b = []
                for entry in file_lines_b:
                    splitline = entry.split(',')
                    splitline.pop(index)
                    entry = ','.join(splitline)
                    print(entry)
                    lines_b += [entry]
                file_lines_b = lines_b
    else:
        split_head_a = header_a.split(',')
        split_head_b = header_b.split(',')

    # Add added/removed heads to string
    if len(removed_heads) > 0:
        det_file_content += 'Removed columns: '
        for head in removed_heads:
            det_file_content += head + ', '
        det_file_content = det_file_content[:-1]
        det_file_content += '\n'
    else:
        det_file_content += 'Removed columns: '
        det_file_content += 'n/a, '
        det_file_content = det_file_content[:-1]
        det_file_content += '\n'

    if len(added_heads) > 0:
        det_file_content += 'Added columns: '
        for head in added_heads:
            det_file_content += head + ', '
        det_file_content = det_file_content[:-1]
        det_file_content += '\n'
    else:
        det_file_content += 'Added columns: '
        det_file_content += 'n/a, '
        det_file_content = det_file_content[:-1]
        det_file_content += '\n'

    # Create a raw string with differences of the two files
    diff_string = [line for line in difflib.Differ().compare(file_lines_a, file_lines_b) if not line.startswith(' ')]

    # parse stringlist to parsable string
    parsed_list = []
    for entry in diff_string:
        if not entry.startswith('?'):
            scrubbed_entry = entry.split(' ', 1)
            if scrubbed_entry[1]:
                entry = csv.reader([scrubbed_entry[1]], skipinitialspace=True)
                entry = entry.__next__()
                scrubbed_entry = [scrubbed_entry[0]] + entry
                parsed_list += [scrubbed_entry]

    # Find specific differences
    while True:
        # If the list is empty, break
        if not parsed_list:
            break

        entry_a = parsed_list[0]

        if len(parsed_list) > 1:
            entry_b = parsed_list[1]

            rest_a = entry_a[2:]
            rest_b = entry_b[2:]
            # Check if the two entries are related and not the same
            if entry_b[1] == entry_a[1] and rest_a != rest_b:
                # Ignoring the first 2 columns (add/remove and identifier), check where the difference is
                for element in range(2, len(entry_a)):
                    if not element == 0 and not element == 1 and element not in removed_heads:
                        if not entry_a[element] == entry_b[element]:
                            # If found make a string and add to result
                            header_a = split_head_a[element - 1]

                            this_string = entry_a[1] + ', ' + header_a + ', "' + entry_a[element] + '", "' + entry_b[element] + '"\n'
                            det_file_content += this_string

                # remove entries from list
                parsed_list.remove(parsed_list[0])
                parsed_list.remove(parsed_list[0])
            elif rest_a != rest_b:
                # Entry is added or removed
                # Determine if added or removed
                if entry_a[0] == '-':
                    this_string = parsed_list[0][1] + ', ' + 'Removed' + ', ' + '|'.join(parsed_list[0]) + ', ' + 'n/a' + '\n'
                    det_file_content += this_string
                elif entry_a[0] == '+':
                    this_string = parsed_list[0][1] + ', ' + 'Added' + ', ' + 'n/a' + ', ' + '|'.join(parsed_list[0]) + '\n'
                    det_file_content += this_string

                # remove entry from list
                parsed_list.remove(parsed_list[0])
            else:
                # Entry is artefact of removed head
                parsed_list.remove(parsed_list[0])
                parsed_list.remove(parsed_list[0])
        else:
            # Entry is added or removed
            # Determine if added or removed
            if entry_a[0] == '-':
                this_string = entry_a[1] + ', ' + 'Removed' + ', ' + '|'.join(entry_a) + ', ' + 'n/a' + '\n'
                det_file_content += this_string
            elif entry_a[0] == '+':
                this_string = entry_a[1] + ', ' + 'Added' + ', ' + 'n/a' + ', ' + '|'.join(entry_a) + '\n'
                det_file_content += this_string

            # remove entry from list
            parsed_list.remove(parsed_list[0])

    export_to_csv(det_file_content, dir_output)
    return det_file_content
# ----------------------------------
# ----------------------------------
def export_to_csv(det_file_content, dir_output):
    # Create export file for results
    if not os.path.isfile(dir_output + 'res_file.csv'):
        with open(dir_output + 'res_file.csv', 'w', newline='') as csvfile:
            # fields = ['Overzicht', 'OnEntryID', 'OnColumn', 'ValueA', 'ValueB']
            # writer = csv.DictWriter(csvfile, fieldnames=fields)
            # writer.writeheader()
            csv.writer(csvfile)

    # Format det_file_contents
    det_file_content = det_file_content.split('\n')

    # put all found differences in csv
    with open(dir_output + 'res_file.csv', 'a', newline='') as csvfile:
        # Write file name
        column1 = 'File: '
        column2 = det_file_content[0]
        csv.writer(csvfile).writerow([column1, column2, ''])
        det_file_content = det_file_content[1:]

        # Write added columns
        splitstring = det_file_content[0].split(':', 1)
        columns = splitstring[1]
        columns = columns[:-1]
        columns = columns[1:]
        csv.writer(csvfile).writerow([splitstring[0] + ':', columns])
        det_file_content = det_file_content[1:]

        # Write removed columns
        splitstring = det_file_content[0].split(':', 1)
        columns = splitstring[1]
        columns = columns[:-1]
        columns = columns[1:]
        csv.writer(csvfile).writerow([splitstring[0] + ':', columns])
        det_file_content = det_file_content[1:]

        # Write entries
        for entry in det_file_content:
            rows = entry.split(',')
            csv.writer(csvfile).writerow(rows)


    # Ensure partition
    with open(dir_output + 'res_file.csv', 'a', newline='') as csvfile:
        csv.writer(csvfile).writerows(['',''])
# ----------------------------------

def find_differences():
    args = parser.parse_args()

    dir_a = 'C:/Users/rik.smolders/Desktop/env-a-files'
    dir_b = 'C:/Users/rik.smolders/Desktop/env-b-files'
    dir_output = 'C:/Users/rik.smolders/Desktop/output_files'
    differences_found = False

    # Create export folder if not exists
    if not os.path.exists(dir_output):
        os.mkdir(dir_output)

    print("finding files")
    files_a = find_files(dir_a)
    files_b = find_files(dir_b)

    det_file_content = ''

    if len(files_a) > 0:
        print("Files present!")

    # Loop trough each file in dir_a and compare it against the same file in dir_b
    for file in files_a:
        print(file)
        file_name, file_ext = os.path.splitext(file)

        output_b_file = os.path.join(dir_b, file)
        output_a_file = os.path.join(dir_a, file)

        # Skip, but notify if file does not exist in output_b
        if not file in files_b:
            print(f'File {file} not found in environment B')
            differences_found = True
            continue

        # Skip if files are identical
        if filecmp.cmp(output_a_file, output_b_file):
            print(f'File {file} identical, skipping')
            continue

        # Export both XML files if a difference is found, they are often too big to  find the specific difference
        if file_ext == '.xml':
            create_export_file(f'{os.path.join(dir_output, file_name)}_a.xml', get_file_content(output_a_file))
            create_export_file(f'{os.path.join(dir_output, file_name)}_b.xml', get_file_content(output_b_file))
            differences_found = True
        else:
            file_a_content = get_file_content(output_a_file)
            file_b_content = get_file_content(output_b_file)

            # Get the lines per file
            file_lines_a = file_a_content.replace('  ', '').splitlines()
            file_lines_b = file_b_content.replace('  ', '').splitlines()

            # Create a html table of the two files
            diff = difflib.HtmlDiff().make_file(file_lines_a, file_lines_b)

            # Compare the 2 stringlists and return a complete html table, showing line by line differences
            output_ab_name = os.path.join(dir_output, file_name + '.html')

            # Count the amount of css elements which highlight differences, count -1 because of the legend
            added = diff.count('class="diff_add"') - 1
            changed = diff.count('class="diff_chg"') - 1
            deleted = diff.count('class="diff_sub"') - 1
            rows = len(file_lines_a) - 1

            print(f'{file}: {rows} rows, {added} added, {changed} changed, {deleted} deleted')

            # Create export if differences are found and rows exist
            if changed + deleted + added > 0 and rows > 0:
                create_export_file(output_ab_name, diff)
                create_export_file(f'{os.path.join(dir_output, file_name)}_a{file_ext}', file_a_content)
                create_export_file(f'{os.path.join(dir_output, file_name)}_b{file_ext}', file_b_content)
                differences_found = True

                # ----------------------------------
                # Add to csv file if diff are found
                # First add name of the file
                det_file_content = file_name
                det_file_content += '\n'

                convert_to_detail_string(file_lines_a, file_lines_b, det_file_content, dir_output)
                # ----------------------------------

                print('End of comparison for this file')


# Temp ------------------------------------------------------------------------
    # Send data to db
    from testsuite_send_to_mongo import send_csv_to_mongo
    send_csv_to_mongo(dir_output, 'res_file.csv')
    # send_csv_to_db(dir_output, 'res_file.csv')

# -----------------------------------------------------------------------------

    # Notify any files that were only present in B
    for file in files_b.difference(files_a):
        print(f'File {file} not found in environment A')
        differences_found = True

    if differences_found:
        raise Exception('Differences found, see artifacts for results')

if __name__ == '__main__':
    find_differences()